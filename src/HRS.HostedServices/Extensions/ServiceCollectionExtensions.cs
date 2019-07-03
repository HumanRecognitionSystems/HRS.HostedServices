using HRS.HostedServices.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HRS.HostedServices.Extensions
{
    /// <summary>
    /// Extensions for adding services to application
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="HostedWarmWebService"/> to an applciation
        /// </summary>
        public static IServiceCollection AddHostedWarmWebService(this IServiceCollection services)
        {
            var configuration = services.BuildServiceProvider().GetService<IConfiguration>();

            var options = configuration.GetSection("HostedWarmWeb").Get<HostedWarmWebOptions>();
            services.AddSingleton(options)
                .AddHostedService<HostedWarmWebService>();

            return services;
        }
    }
}
