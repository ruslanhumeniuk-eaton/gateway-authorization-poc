using System.Security.Claims;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.Extensions.Caching.Memory;

namespace YarpReversedProxy.Authorization;

public class PermissionService : IPermissionService
{
    private readonly IMemoryCache _cache;
    private readonly MemoryCacheEntryOptions _cacheEntryOptions;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly TimeSpan _slidingExpirationPeriod = TimeSpan.FromSeconds(60);

    public PermissionService(IMemoryCache cache, IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _cache = cache;
        _cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(_slidingExpirationPeriod);
    }

    public async Task<bool> IsAuthorizedAsync(ClaimsPrincipal user, string requiredKey)
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

        HashSet<string> permissions = await GetUserPermissions(userId);

        return permissions.Contains(requiredKey);
    }

    private Task<HashSet<string>> GetUserPermissions(string userId) =>
        _cache.GetOrCreateAsync(GenerateCacheKey(userId), CreateCacheEntry());

    private static string GenerateCacheKey(string userId) => $"AuthCacheKey-{userId}";

    private Func<ICacheEntry, Task<HashSet<string>>> CreateCacheEntry() => async entry =>
    {
        // perform a request to UserAccess service here.
        HttpClient client = _httpClientFactory.CreateClient("user-access");
        //client.SetBearerToken("");

        var userPermissions = await client.GetFromJsonAsync<UserPermissions>("api/permissions");

        entry.SetOptions(_cacheEntryOptions);

        return userPermissions.Permissions;
    };
}