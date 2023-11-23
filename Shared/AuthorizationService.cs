using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using Etds.BuildingBlocks.Infrastructure.Contexts;
using IdentityModel;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Shared;

public class AuthorizationService : IAuthorizationService
{
    private const string AuthorizationCacheKey = "Authorization";

    private readonly IMemoryCache _cache;
    private readonly MemoryCacheEntryOptions _cacheEntryOptions;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _slidingExpirationPeriod = TimeSpan.FromSeconds(60);

    public AuthorizationService(IMemoryCache cache, IHttpClientFactory httpClientFactory, IServiceProvider serviceProvider)
    {
        _httpClientFactory = httpClientFactory; 
        _serviceProvider = serviceProvider;
        _cache = cache;
        _cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(_slidingExpirationPeriod);
    }

    public async Task<bool> IsAuthorizedAsync(ClaimsPrincipal user, string permissionKey, string accessToken)
    {
        string userId = user
            .Claims
            .SingleOrDefault(x => x.Type == JwtClaimTypes.Subject)?
            .Value;

        if (string.IsNullOrWhiteSpace(userId))
        {
            //return false; // return false in a real case
            userId = Guid.NewGuid().ToString();
        }

        HashSet<string> permissions = await GetUserPermissionsAsync(userId, accessToken);

        return permissions.Contains(permissionKey);
    }

    public async Task<bool> IsAuthorizedAsync(string permissionKey, string organizationId)
    {
        Guid? userId = UserId();
        string? accessToken = AccessToken();

        if (!userId.HasValue || userId.Value == default)
        {
            //return false; // return false in a real case
            userId = Guid.NewGuid();
        }

        HashSet<string> permissions = await GetUserPermissionsAsync(userId.Value.ToString(), accessToken, organizationId);

        return permissions.Contains(permissionKey);
    }

    private Task<HashSet<string>> GetUserPermissionsAsync(string userId, string accessToken, string? organizationId = null) =>
        _cache.GetOrCreateAsync(GenerateCacheKey(userId, organizationId), CreateCacheEntry(accessToken, organizationId));

    private static string GenerateCacheKey(string userId, string? organizationId) => $"{AuthorizationCacheKey}-{userId}{(organizationId is not null ? $"-{organizationId}" : "")}";

    private Func<ICacheEntry, Task<HashSet<string>>> CreateCacheEntry(string accessToken, string? organizationId) => async entry =>
    {
        // perform a request to UserAccess service here.
        HttpClient client = _httpClientFactory.CreateClient("user-access");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken); // new AuthenticationHeaderValue("Bearer", "Your Oauth token");

        var userPermissions = await client.GetFromJsonAsync<UserPermissions>($"api/permissions?organizationId={organizationId}");

        entry.SetOptions(_cacheEntryOptions);

        return userPermissions.Permissions;
    };

    private Guid? UserId() => _serviceProvider.GetRequiredService<IContext>().Identity?.Id;
    
    private string? AccessToken() => _serviceProvider.GetRequiredService<IContext>().Identity?.Token;
}