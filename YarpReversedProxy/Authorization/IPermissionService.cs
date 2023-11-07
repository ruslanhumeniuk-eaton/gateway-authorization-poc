using System.Security.Claims;

namespace YarpReversedProxy.Authorization
{
    public interface IPermissionService
    {
        Task<bool> IsAuthorizedAsync(ClaimsPrincipal user, string requiredKey);
    }
}