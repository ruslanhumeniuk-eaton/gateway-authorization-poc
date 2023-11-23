using System.Text.Json.Serialization;
using OcelotGateway.Hateoas.Serialization;
using OcelotGateway.Ocelot;
using Shared.Serialization;

namespace OcelotGateway.Hateoas;

internal static class Extensions
{
    internal static IServiceCollection AddHateoas(this IServiceCollection services)
    {
        services.AddSingleton<IJsonConverterFactory, HateoasJsonConverterFactory>();
        services.ConfigureOptions<ConfigureHateoasMvcOptions>();
        services.AddSingleton<IRouteCollection>(serviceProvider =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var routeConfigs = configuration.GetSection("Routes").Get<List<OcelotRouteConfiguration>>();
            return new RouteCollection(routeConfigs);
        });

        services.Scan(scan => scan
            .FromEntryAssembly()
            .AddClasses(c => c.AssignableTo(typeof(IHateoasLinksConfiguration<>)), false)
            .AsImplementedInterfaces()
            .WithSingletonLifetime());

        return services;
    }
}