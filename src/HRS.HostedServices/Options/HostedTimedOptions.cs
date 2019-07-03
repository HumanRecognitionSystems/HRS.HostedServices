using System;

namespace HRS.HostedServices.Options
{
    /// <summary>
    /// Options for the <see cref="HostedTimedService"/>
    /// </summary>
    public class HostedTimedOptions
    {
        /// <summary>
        /// Inital Delay before first execution
        /// </summary>
        public TimeSpan InitialDelay { get; set; } = TimeSpan.FromSeconds(1);

        /// <summary>
        /// The interval between calls
        /// </summary>
        public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(1);
    }
}
