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
        [HttpGet(".links")]
        public ActionResult<ContractMeta> GetMetalinks() => Ok(new ContractMeta());

        [HttpGet("{contractId}")]
        public ActionResult<ContractDto> Get([FromRoute] ResourceId<Guid> contractId)
        {
            return Ok(new ContractDto
            {
                Id = Guid.NewGuid(),
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
                Address = new AddressDto
                {
                    Line1 = "Address 1",
                    Line2 = "Address Subline 2",
                    AcceptedCities = new[] { new CityDto { Code = 143, Name = "Tourists City" } }
                }
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
                    Id = Guid.NewGuid(),
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Address = new AddressDto
                    {
                        Line1 = $"Address Line 1{Random.Shared.Next(20, 55)}",
                        Line2 = $"Address Subline 2{Random.Shared.Next(20, 55)}",
                        AcceptedCities = new[]
                        {
                            new CityDto
                            {
                                Code = Random.Shared.Next(100, 155),
                                Name = $"Tourists City {Random.Shared.Next(800, 879)}"
                            }
                        }
                    }
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