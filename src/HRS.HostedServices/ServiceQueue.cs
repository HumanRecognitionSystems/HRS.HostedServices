using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace HRS.HostedServices
{
    /// <summary>
    /// Stnadard implementation of <see cref="IServiceQueue{T}"/> for <see cref="HostedQueueService{T}"/>
    /// </summary>
    /// <typeparam name="T">The type of items to queue</typeparam>
    public class ServiceQueue<T> : IServiceQueue<T>
    {
        private readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);

        /// <summary>
        /// The number of items in the queue
        /// </summary>
        /// <remarks>
        /// The queue is concurrent so the count should be treated with suspicion
        /// </remarks>
        public int Count => _queue.Count;

        /// <summary>
        /// Checks to see if the queue is empty
        /// </summary>
        /// <remarks>
        /// The queue is concurrent so the isempty should be treated with suspicion
        /// </remarks>
        public bool IsEmpty => _queue.IsEmpty;

        /// <summary>
        /// Places an item on the item
        /// </summary>
        public void Enqueue(T item)
        {
            if (item != null)
            {
                _queue.Enqueue(item);
                _signal.Release();
            }
        }

        /// <summary>
        /// Removes an item from the queue
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token to stop waiting for the queue</param>
        public async Task<T> Dequeue(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            if(_queue.TryDequeue(out var item))
            {
                return item;
            }

            return default;
        }
    }
}
