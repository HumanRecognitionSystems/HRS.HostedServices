using System;

namespace HRS.HostedServices.Options
{
    /// <summary>
    /// The options for the <see cref="HostedQueueService{T}"/>
    /// </summary>
    public class HostedQueueOptions
    {
        /// <summary>
        /// Requeue items if an exception is throw defaults to false, which causes the item to be throw away
        /// </summary>
        public bool RequeueFailedItems { get; set; }

        /// <summary>
        /// Requeue delay the amount of time before the item is requeued
        /// </summary>
        public TimeSpan RequeueDelay { get; set; } = TimeSpan.FromMilliseconds(100);

        /// <summary>
        /// If there should be delay between processing items in the queue
        /// </summary>
        public TimeSpan NextItemDelay { get; set; } = TimeSpan.Zero;
    }
}
