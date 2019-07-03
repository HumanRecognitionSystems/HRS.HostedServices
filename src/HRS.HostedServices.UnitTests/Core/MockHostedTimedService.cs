using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HRS.HostedServices.Options;
using Microsoft.Extensions.Logging;

namespace HRS.HostedServices.UnitTests.Core
{
    public class MockHostedTimedService : HostedTimedService
    {
        private readonly Func<Task> _execute;

        public MockHostedTimedService(Func<Task> execute, HostedTimedOptions options, ILogger logger) : base(options, logger)
        {
            _execute = execute;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken) => _execute();
    }
}
