using HRS.HostedServices.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HRS.HostedServices
{
    /// <summary>
    /// Abstract class for Timed Service
    /// </summary>
    public abstract class HostedTimedService : IHostedService, IDisposable
    {
        private readonly HostedTimedOptions _options;
        private readonly Timer _timer;
        private readonly ILogger _logger;

        private Task _executingTask;
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

        /// <summary>
        /// Constructor for timed service
        /// </summary>
        /// <param name="options">The options for the service</param>
        /// <param name="logger">The logger for the service</param>
        public HostedTimedService(HostedTimedOptions options, ILogger logger)
        {
            _options = options;
            _logger = logger;

            _timer = new Timer(ExecuteTask, null, Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// Start service
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to cancel the starting service</param>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer.Change(_options.InitialDelay, TimeSpan.FromMilliseconds(-1));
            return Task.CompletedTask;
        }

        /// <summary>
        /// Stops the service
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to cancel the stopping of the service</param>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Change(Timeout.Infinite, 0);

            if (_executingTask == null)
            {
                return;
            }

            try
            {
                _tokenSource.Cancel();
            }
            finally
            {
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }
        }

        private void ExecuteTask(object state)
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            _executingTask = Task.Run(async () =>
            {
                try
                {
                    await ExecuteAsync(_tokenSource.Token);
                }
                finally
                {
                    _timer.Change(_options.Interval, TimeSpan.FromMilliseconds(-1));
                }
            }).ContinueWith(t =>
            {
                try
                {
                    t.Wait();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to Execute task");
                }
            }, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted);
        }

        /// <summary>
        /// The function to call at the interval
        /// </summary>
        /// <param name="stoppingToken">The token to stop processing</param>
        protected abstract Task ExecuteAsync(CancellationToken stoppingToken);

        /// <summary>
        /// Disposes of the service
        /// </summary>
        public virtual void Dispose()
        {
            _timer.Dispose();
        }
    }
}
