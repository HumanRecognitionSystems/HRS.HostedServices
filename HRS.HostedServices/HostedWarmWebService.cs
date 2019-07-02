using HRS.HostedServices.Options;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HRS.HostedServices
{
    /// <summary>
    /// A service to call endpoints on a schedule to keep them warm
    /// </summary>
    public class HostedWarmWebService : HostedTimedService
    {
        private readonly HostedWarmWebOptions _options;
        private readonly ILogger _logger;
        private readonly Func<HttpClient> _getClient;
        private HttpClient _client;

        /// <summary>
        /// Constructs warm web service
        /// </summary>
        /// <param name="options">The options for the warm web service</param>
        /// <param name="logger">The logger for the service</param>
        /// <param name="getClient">A function for getting the httpclient, will use the default if null</param>
        public HostedWarmWebService(HostedWarmWebOptions options, ILogger<HostedWarmWebService> logger, Func<HttpClient> getClient = null) : base(options, logger)
        {
            _options = options;
            _logger = logger;

            if(getClient == null)
            {
                _client = new HttpClient
                {
                    BaseAddress = _options.BaseUri
                };
            }
            _getClient = getClient;
        }

        /// <summary>
        /// Calls the endpoints
        /// </summary>
        /// <param name="stoppingToken">The cancellation token to stop executing</param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if(_client == null)
            {
                _client = _getClient();
            }

            var tasks = new List<Task>();
            foreach (var endpoint in _options.WarmupEndpoints)
            {
                tasks.Add(_client.GetAsync(endpoint, stoppingToken));
            }

            try
            {
                await Task.WhenAll(tasks);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error while touching the endpoints");
            }
        }

        /// <summary>
        /// Disposes of the service
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            _client?.Dispose();
        }
    }
}
