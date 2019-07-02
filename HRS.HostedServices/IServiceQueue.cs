using System.Threading;
using System.Threading.Tasks;

namespace HRS.HostedServices
{
    /// <summary>
    /// Interface for queue used by <see cref="HostedQueueService{T}"/>
    /// </summary>
    /// <typeparam name="T">The type of items to queue</typeparam>
    public interface IServiceQueue<T>
    {
        /// <summary>
        /// Places an item on the item
        /// </summary>
        void Enqueue(T item);

        /// <summary>
        /// Removes an item from the queue
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token to stop waiting for the queue</param>
        Task<T> Dequeue(CancellationToken cancellationToken);
    }
}
