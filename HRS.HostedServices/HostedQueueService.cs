using HRS.HostedServices.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HRS.HostedServices
{
    /// <summary>
    /// Abstract class for a hosted queueing service
    /// </summary>
    /// <typeparam name="T">The type of items to queue</typeparam>
    public abstract class HostedQueueService<T> : BackgroundService
    {
        private readonly IServiceQueue<T> _queue;
        private readonly ILogger _logger;
        private readonly HostedQueueOptions _options;

        /// <summary>
        /// Constructor for hosted queues
        /// </summary>
        /// <param name="queue">The queue to process</param>
        /// <param name="logger">A standard logger to use for reporting errors</param>
        /// <param name="options">The options to use for this hosted queue</param>
        public HostedQueueService(IServiceQueue<T> queue, ILogger logger, HostedQueueOptions options)
        {
            _queue = queue;
            _logger = logger;
            _options = options;
        }

        /// <summary>
        /// Main loop for dequeue and begining the processing of items
        /// </summary>
        /// <param name="stoppingToken">Cancellation Token to stop waiting for queued items</param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var item = await _queue.Dequeue(stoppingToken);
                try
                {
                    await ProcessItem(item);
                }
                catch (Exception ex)
                {
                    if (_options.RequeueFailedItems)
                    {
                        _logger.LogWarning(ex, "Failed to process Item - Requeue Enabled - Adding to back of queue");
                        RequeueItem(item);
                    }
                    else
                    {
                        _logger.LogError(ex, "Failed to process queued item - REQUEUE NOT ENABLED");
                    }
                }
                await Task.Delay(_options.NextItemDelay);
            }
        }

        /// <summary>
        /// The main process item task to override. will pass in one item at a time 
        /// </summary>
        /// <param name="item">The item to process</param>
        protected abstract Task ProcessItem(T item);

        private void RequeueItem(T item)
        {
            if (_options.RequeueDelay != TimeSpan.Zero)
            {
                Task.Run(async () =>
                {
                    await Task.Delay(_options.RequeueDelay);
                    _queue.Enqueue(item);
                }).ContinueWith(t =>
                    {
                        try
                        {
                            t.Wait();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to requeue item after a previous failure");
                        }
                    }, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted);
            }
            else
            {
                _queue.Enqueue(item);
            }
        }
    }
}
