using System.Security.Claims;

namespace Shared
{
    public interface IPermissionService
    {
        Task<bool> IsAuthorizedAsync(ClaimsPrincipal user, string permissionKey, string? accessToken);
    }
}