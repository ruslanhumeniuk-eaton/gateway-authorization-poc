using System.Security.Claims;

namespace OcelotGateway.Authorization
{
    public interface IPermissionService
    {
        Task<bool> IsAuthorizedAsync(ClaimsPrincipal user, string requiredKey);
    }
}