using Microsoft.AspNetCore.Authentication;
using Shared;

namespace YarpReversedProxy.Authorization
{
    public class AuthorizationMiddleware : IMiddleware
    {
        private readonly IConfiguration _configuration;
        private readonly IPermissionService _permissionService;

        public AuthorizationMiddleware(IConfiguration configuration, IPermissionService permissionService)
        {
            _configuration = configuration;
            _permissionService = permissionService;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!IsAuthenticated(context))
            {
                context.Response.StatusCode = 403;
                return;
            }

            RouteConfiguration? route = GetYarpRouteConfiguration(context);
            if (route == null)
            {
                context.Response.StatusCode = 404;
                return;
            }

            if (RequirePermission(route.Permission))
            {
                var token = await context.GetTokenAsync("access_token");
                bool isAuthorized = await _permissionService. IsAuthorizedAsync(context.User, route.Permission, token);

                if (!isAuthorized)
                {
                    context.Response.StatusCode = 403;
                    return;
                }
            }

            await next(context);
        }

        private static bool IsAuthenticated(HttpContext context) =>
            context.User.Identity?.IsAuthenticated == true;

        private static bool RequirePermission(string permission) =>
            !string.IsNullOrWhiteSpace(permission)
            && permission != "NoPermissionRequired"
            && permission != "HasDriverPermission";

        private RouteConfiguration? GetYarpRouteConfiguration(HttpContext context)
        {
            // cache using memory cache or dictionary
            string? endpointPath = context.Request.Path.Value;
            var routeConfigs = _configuration.GetSection("ReverseProxy").GetSection("Routes").Get<List<RouteConfiguration>>();
            return routeConfigs.FirstOrDefault(r => r.GetEndpointPath == endpointPath);
        }
    }
}