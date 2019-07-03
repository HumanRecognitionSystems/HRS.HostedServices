using HRS.HostedServices.Options;
using HRS.HostedServices.UnitTests.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HRS.HostedServices.UnitTests
{
    public class WarmupControllerTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public WarmupControllerTests(ITestOutputHelper testOutputHelper)
        {
            _outputHelper = testOutputHelper;
        }

        [Fact]
        public async Task TestControllerWebWarm()
        {
            TestServer server = null;
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseMvc();
                })
                .ConfigureServices(services => {
                    services.AddMvcCore().AddFormatterMappings().AddJsonFormatters();
                    services.AddSingleton<ILogger<HostedWarmWebService>>(sp => new XUnitTestLoggerNamed<HostedWarmWebService>(_outputHelper));
                    var options = new HostedWarmWebOptions
                    {
                        BaseUri = new Uri("http://localhost/"),
                        WarmupEndpoints = new [] { "testserver/countcalls", "testserver/badendpoint" },
                        Interval = TimeSpan.FromSeconds(2),
                        InitialDelay = TimeSpan.FromMilliseconds(500)
                    };
                    services.AddSingleton(options);
                    var client = new HttpClient { BaseAddress = new Uri("http:") };
                    services.AddSingleton<Func<HttpClient>>(sp => () => server.CreateClient());
                    services.AddHostedService<HostedWarmWebService>();
                });

            using(server = new TestServer(builder))
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
            }

            Assert.InRange(TestServerController.Calls, 2, 4);
        }
    }
}
