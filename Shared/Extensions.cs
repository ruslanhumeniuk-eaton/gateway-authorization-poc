using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Shared.Encryption;
using Shared.ResourceIdConfiguration;

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
            services.AddSingleton<IAuthorizationService, AuthorizationService>();
            services.AddHttpClient("user-access", client => { client.BaseAddress = new Uri("https://localhost:7278"); });
            return services;
        }

        public static IServiceCollection AddEncryption(this IServiceCollection services)
        {
            services.AddSingleton<IEncryptor>(x => new Encryptor("aesKey", "aesInitializationVector"));

            return services;
        }

        public static IServiceCollection AddIdorConfiguration(this IServiceCollection services)
        {
            services.AddResourceIdConfiguration();

            return services;
        }
    }
}