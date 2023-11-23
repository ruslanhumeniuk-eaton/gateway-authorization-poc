using Microsoft.AspNetCore.Authentication;
using OcelotGateway.Ocelot;
using Shared;

namespace OcelotGateway.Authorization
{
    public class AuthorizationMiddleware : IMiddleware
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthorizationService _authorizationService;

        public AuthorizationMiddleware(IConfiguration configuration, IAuthorizationService authorizationService)
        {
            _configuration = configuration;
            _authorizationService = authorizationService;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            OcelotRouteConfiguration? route = GetOcelotRouteConfiguration(context);
            if (route == null)
            {
                context.Response.StatusCode = 404;
                return;
            }

            if (route.AuthenticationOptions is not null)
            {
                if (!IsAuthenticated(context))
                {
                    context.Response.StatusCode = 403;
                    return;
                }

                if (RequirePermission(route.PermissionKeys))
                {
                    string? token = await context.GetTokenAsync("access_token");

                    foreach (string permission in route.PermissionKeys)
                    {
                        bool isAuthorized = await _authorizationService.IsAuthorizedAsync(context.User, permission, token);

                        if (!isAuthorized)
                        {
                            context.Response.StatusCode = 403;
                            return;
                        }
                    }
                }
            }

            await next(context);
        }

        private static bool IsAuthenticated(HttpContext context) =>
            context.User.Identity?.IsAuthenticated == true;

        private static bool RequirePermission(string[]? permissions) =>
            permissions is not null && permissions.Any()
            && permissions.All(p => p != "NoPermissionRequired")
            && permissions.All(p => p != "HasDriverPermission");

        private OcelotRouteConfiguration? GetOcelotRouteConfiguration(HttpContext context)
        {
            // cache using memory cache or dictionary
            string? endpointPath = context.Request.Path.Value;
            var routeConfigs = _configuration.GetSection("Routes").Get<List<OcelotRouteConfiguration>>();
            return routeConfigs.FirstOrDefault(r => r.UpstreamPathTemplate == endpointPath);
        }
    }
}