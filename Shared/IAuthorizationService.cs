using System.Security.Claims;

namespace Shared
{
    public interface IAuthorizationService
    {
        Task<bool> IsAuthorizedAsync(ClaimsPrincipal user, string permissionKey, string accessToken);

        Task<bool> IsAuthorizedAsync(string permissionKey, string organizationId);
    }
}