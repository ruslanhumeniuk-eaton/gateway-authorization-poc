using Contracts;
using Contracts.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MicroApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContractController : ControllerBase
    {
        private static readonly string[] Summaries =
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet(".links")]
        public ActionResult<ContractMeta> GetMetalinks() => Ok(new ContractMeta());

        [HttpGet("{contractId}")]
        public ActionResult<ContractDto> Get([FromRoute] ResourceId<Guid> contractId)
        {
            return Ok(new ContractDto
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
                RandomNumber = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            });
        }

        [Authorize]
        [HttpGet]
        public ActionResult<ContractListDto> GetContracts()
        {
            return Ok(new ContractListDto
            {
                Contracts = Enumerable.Range(1, 5).Select(index => new ContractDto
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    RandomNumber = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                })
            });
        }

        [Authorize]
        [HttpPut]
        public ActionResult Put()
        {
            return Ok();
        }

        [Authorize]
        [HttpDelete]
        public ActionResult Delete()
        {
            return NoContent();
        }
    }
}