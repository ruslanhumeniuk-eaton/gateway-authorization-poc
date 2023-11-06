using ContractsService;
using Microsoft.AspNetCore.Mvc;

namespace MicroApi.Controllers
{
    [ApiController]
    [Route("api")]
    public class DownstreamServiceController : ControllerBase
    {
        private static readonly string[] Summaries =
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet("contracts")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                })
                .ToArray();
        }

        [HttpGet("personal-data")]
        public object GetData() => new { time = DateTime.Now, Age = Random.Shared.Next(18, 81) };

        [HttpGet("no-permission")]
        public object GetNoPermissionData() => new { time = DateTime.Now, Label = "You got data from an endpoint with NoPermission" };

        [HttpGet("driver-permission")]
        public object GetDriverPermission() => new { time = DateTime.Now, Label = "You got data from an endpoint with HasDriverPermission" };

        [HttpGet("get-future")]
        public object GetFuture() => new { time = DateTime.Now, Label = "I have no idea how you can see it. smt broken" };
    }
}