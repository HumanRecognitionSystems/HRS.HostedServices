using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HRS.HostedServices.Options;
using Microsoft.Extensions.Logging;

namespace HRS.HostedServices.UnitTests.Core
{
    internal class MockHostedQueueService<T> : HostedQueueService<T>
    {
        private readonly Func<T, Task> _processItem;

        public MockHostedQueueService(Func<T, Task> processItem, IServiceQueue<T> queue, ILogger logger, HostedQueueOptions options) : base(queue, logger, options)
        {
            _processItem = processItem;
        }

        protected override Task ProcessItem(T item) => _processItem(item);
    }
}
