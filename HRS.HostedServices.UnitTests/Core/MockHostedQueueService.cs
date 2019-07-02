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
        public MockHostedQueueService(Func<T, Task> itemHandler, IServiceQueue<T> queue, ILogger logger, HostedQueueOptions options) : base(queue, logger, options)
        {
            ItemHandler = itemHandler;
        }

        public Func<T, Task> ItemHandler { get; set; }

        protected override Task ProcessItem(T item) => ItemHandler(item);
    }
}
