using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Shared
{
    public static class Extensions
    {
        public static IServiceCollection AddGreenMotionIdentityServerAuthentication(this IServiceCollection services)
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(o =>
                {
                    o.Authority = "https://login.dev.greenmotion.tech";

                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false
                    };
                });

            return services;
        }

        public static IServiceCollection AddPermissionServices(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton<IPermissionService, PermissionService>();
            services.AddHttpClient("user-access", client => { client.BaseAddress = new Uri("https://localhost:7278"); });
            return services;
        }
    }
}