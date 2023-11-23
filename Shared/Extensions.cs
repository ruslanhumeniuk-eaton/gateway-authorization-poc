using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using Shared.Encryption;
using Shared.ResourceIdConfiguration;
using Shared.Serialization;

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

        /// <summary>
        ///     Add JSON configuration.
        /// </summary>
        /// <param name="services">Collection of service descriptors.</param>
        /// <returns>Collection of service descriptors for chaining.</returns>
        public static IServiceCollection AddSerialization(this IServiceCollection services)
        {
            services.AddSingleton<IContractResolver, JsonContractResolver>();
            services.ConfigureOptions<ConfigureJsonOptions>();
            
            return services;
        }

        public static IServiceCollection AddEncryption(this IServiceCollection services)
        {
            services.AddSingleton<IEncryptor>(x => new Encryptor("H4jLwp2pwNxArTa4cWKmI/Pgh97QKzvrOjQH4xwUCw0=", "ty/fjnM+jkAhguR0ZYuuJA=="));

            return services;
        }

        public static IServiceCollection AddIdorConfiguration(this IServiceCollection services)
        {
            services.AddResourceIdConfiguration();

            return services;
        }
    }
}