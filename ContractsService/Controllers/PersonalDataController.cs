using Contracts;
using Contracts.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MicroApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonalDataController : ControllerBase
    {
        [HttpGet(".links")]
        public ActionResult<PersonalDataMeta> GetMetalinks([FromHeader, Required] ResourceId<Guid> contractId) =>
            Ok(new PersonalDataMeta(contractId));

        [HttpGet]
        public ActionResult<PersonalDataDto> Get() => Ok(new PersonalDataDto { Note = "example" });

        [HttpGet("byContract")]
        public ActionResult<PersonalDataDto> GetByContract([FromQuery] CollectionQueryParameters parameters,
            [FromHeader, Required] ResourceId<Guid> contractId) => Ok(new PersonalDataDto
            { ContractId = contractId.Value, Note = "the one to test query/route parameters" });

        [Authorize]
        [HttpGet("no-permission")]
        public ActionResult<PersonalDataDto> GetNoPermissionData() => Ok(new PersonalDataDto
            { Note = "You got data from an endpoint with NoPermission" });

        [Authorize]
        [HttpGet("driver-permission")]
        public ActionResult<PersonalDataDto> GetDriverPermission() => Ok(new PersonalDataDto
            { Note = "You got data from an endpoint with HasDriverPermission" });

        [Authorize]
        [HttpGet("get-future")]
        public ActionResult<PersonalDataDto> GetFuture() => Ok(new PersonalDataDto
            { Note = "I have no idea how you can see it. smt broken" });
    }
}