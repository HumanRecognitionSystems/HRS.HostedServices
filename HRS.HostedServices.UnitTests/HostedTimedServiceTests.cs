using HRS.HostedServices.Options;
using HRS.HostedServices.UnitTests.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HRS.HostedServices.UnitTests
{
    public class HostedTimedServiceTests
    {
        private readonly ITestOutputHelper _outputHelper;
        private readonly ILogger _logger;

        public HostedTimedServiceTests(ITestOutputHelper testOutputHelper)
        {
            _outputHelper = testOutputHelper;
            _logger = new XUnitTestLogger(_outputHelper, "TimeService");
        }

        [Fact]
        public async Task TestTimedSimple()
        {
            var totalCount = 0;
            
            Func<Task> execute = () =>
            {
                totalCount++;
                return Task.CompletedTask;
            };

            await RunTest(execute);
            Assert.Equal(5, totalCount);
        }

        [Fact]
        public async Task TestTimedWithSingleException()
        {
            var totalCount = 0;
            var errorCount = 0;


            Func<Task> execute = () =>
            {
                totalCount++;
                if(totalCount == 2)
                {
                    errorCount++;
                    throw new NotSupportedException();
                }
                return Task.CompletedTask;
            };

            await RunTest(execute);
            Assert.Equal(5, totalCount);
            Assert.Equal(1, errorCount);
        }

        [Fact]
        public async Task TestTimedWithDelayedAction()
        {
            var totalCount = 0;

            Func<Task> execute = async () =>
            {
                totalCount++;
                await Task.Delay(50);
            };

            await RunTest(execute);
            Assert.Equal(3, totalCount);
        }

        [Fact]
        public async Task TestTimedWithDelayActionAndException()
        {
            var totalCount = 0;
            var errorCount = 0;

            Func<Task> execute = async () =>
            {
                totalCount++;
                await Task.Delay(50);
                if(totalCount == 2)
                {
                    errorCount++;
                    throw new NotSupportedException();
                }
            };

            await RunTest(execute);
            Assert.Equal(3, totalCount);
            Assert.Equal(1, errorCount);
        }

        private async Task RunTest(Func<Task> execute)
        {
            var options = new HostedTimedOptions { InitialDelay = TimeSpan.FromMilliseconds(50), Interval = TimeSpan.FromMilliseconds(100) };
            using (var service = new MockHostedTimedService(execute, options, _logger))
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2)))
            {
                await service.StartAsync(cts.Token);
                await Task.Delay(500);
                await service.StopAsync(cts.Token);
            }
        }
    }
}
