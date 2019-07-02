using HRS.HostedServices.Options;
using HRS.HostedServices.UnitTests.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HRS.HostedServices.UnitTests
{
    public class HostedQueuedServiceTests
    {
        private const string InvalidItem = "Invalid";
        private const string ValidItem = "Valid";
        private readonly ILogger _logger;

        public HostedQueuedServiceTests(ITestOutputHelper testOutputHelper)
        {
            _logger = new XUnitTestLogger(testOutputHelper, "HostedQueue");
        }

        private static ServiceQueue<string> GetQueue()
        {
            var queue = new ServiceQueue<string>();
            queue.Enqueue(InvalidItem);
            queue.Enqueue(ValidItem);
            return queue;
        }

        [Fact]
        public async Task WhenAQueuedTaskThrowsAnExceptionThenQueueContinues()
        {
            var queue = GetQueue();
            var processedItems = new List<string>();

            Task ProcessItem(string item)
            {
                if(item == InvalidItem)
                {
                    throw new NotSupportedException();
                }
                processedItems.Add(item);
                return Task.CompletedTask;
            }

            using(var service = new MockHostedQueueService<string>(ProcessItem, queue, _logger, new HostedQueueOptions()))
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2)))
            {
                await service.StartAsync(cts.Token);
                await Task.Delay(100);
                await service.StopAsync(cts.Token);
                Assert.Equal(ValidItem, Assert.Single(processedItems));
                Assert.True(queue.IsEmpty);
            }
        }

        [Fact]
        public async Task WhenAQueuedTaskThrowsAnExceptionAndRequeueIsEnabledTheItemGetsReprocessed()
        {
            var queue = GetQueue();
            var processedItems = new List<string>();
            var invalidItems = new List<string>();

            Task ProcessItem(string item)
            {
                if(item == InvalidItem)
                {
                    invalidItems.Add(item);
                    if(invalidItems.Count < 2)
                    {
                        throw new NotSupportedException();
                    }
                }
                else
                {
                    processedItems.Add(item);
                }
                return Task.CompletedTask;
            }

            var options = new HostedQueueOptions
            {
                RequeueFailedItems = true
            };

            using(var service = new MockHostedQueueService<string>(ProcessItem, queue, _logger, options))
            using(var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2)))
            {
                await service.StartAsync(cts.Token);
                await Task.Delay(100);
                await service.StopAsync(cts.Token);
                Assert.Equal(ValidItem, Assert.Single(processedItems));
                Assert.Equal(2, invalidItems.Count);
                foreach (var item in invalidItems)
                {
                    Assert.Equal(InvalidItem, item);
                }
                Assert.True(queue.IsEmpty);
            }
        }

        [Fact]
        public async Task WhenAQueuedTaskThrowsAnExceptionAndRequeueIsDisabledThemItemGetsDropped()
        {
            var queue = GetQueue();
            var processedItems = new List<string>();
            var invalidItems = new List<string>();

            Task ProcessItem(string item)
            {
                if (item == InvalidItem)
                {
                    invalidItems.Add(item);
                    throw new NotSupportedException();
                }
                processedItems.Add(item);
                return Task.CompletedTask;
            }

            var options = new HostedQueueOptions
            {
                RequeueFailedItems = false
            };
            using (var service = new MockHostedQueueService<string>(ProcessItem, queue, _logger, options))
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2)))
            {
                await service.StartAsync(cts.Token);
                await Task.Delay(100);
                await service.StopAsync(cts.Token);
                Assert.Equal(ValidItem, Assert.Single(processedItems));
                Assert.Equal(InvalidItem, Assert.Single(invalidItems));
                Assert.True(queue.IsEmpty);
            }
        }
    }
}
