using Microsoft.AspNetCore.Mvc;

namespace AuthorizationService.Controllers
{
    [ApiController]
    [Route("api")]
    public class PermissionsController : ControllerBase
    {
        private static readonly HashSet<string> Permissions = new(new[] { "test", "CanGetContracts" });

        [HttpGet("permissions")]
        public UserPermissions Get() => new()
        {
            Permissions = Permissions
        };
    }
}