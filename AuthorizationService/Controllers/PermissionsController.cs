using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace AuthorizationService.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api")]
    public class PermissionsController : ControllerBase
    {
        private static readonly HashSet<string> Permissions = new(new[] { "test", "CanGetContracts", "CanUpdateContract" });

        [HttpGet("permissions")]
        public UserPermissions Get() => new()
        {
            Permissions = Permissions
        };
    }
}