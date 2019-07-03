using System;
using System.Collections.Generic;
using System.Linq;

namespace HRS.HostedServices.Options
{
    /// <summary>
    /// Options for the <see cref="HostedWarmWebService"/>
    /// </summary>
    public class HostedWarmWebOptions : HostedTimedOptions
    {
        /// <summary>
        /// The base uri to use for the endpoints if left blank the endpoints must be complete
        /// </summary>
        public Uri BaseUri { get; set; }

        /// <summary>
        /// The list of endpoints to keep warm
        /// </summary>
        public IEnumerable<string> WarmupEndpoints { get; set; } = Enumerable.Empty<string>();
    }
}
