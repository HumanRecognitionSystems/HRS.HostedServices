using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HRS.HostedServices.UnitTests.Core
{
    [ApiController, Route("testserver")]
    public class TestServerController : ControllerBase
    {
        private static int _callCount = 0;

        [HttpPost, Route("executedelay")]
        public async Task<IActionResult> ExecuteDelay(Delay delay)
        {
            _callCount++;
            await Task.Delay(delay.Interval);
            return Ok();
        }

        [HttpGet, Route("countcalls")]
        public IActionResult CountCall()
        {
            _callCount++;
            return Ok();
        }

        public static int Calls => _callCount;
    }
}
