using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uhost.Core.Services.Scheduler;

namespace Uhost.Web.Controllers
{
    [Route("api/v2/test"), AllowAnonymous]
    public class TestController : Controller
    {
        private readonly ISchedulerService _schedule;

        public TestController(ISchedulerService schedule)
        {
            _schedule = schedule;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _schedule.ScheduleTest();

            return Ok();
        }
    }
}
