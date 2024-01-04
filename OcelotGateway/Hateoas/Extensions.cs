using System.Text.Json.Serialization;
using OcelotGateway.Hateoas.Serialization;
using OcelotGateway.Ocelot;
using HateoasJsonConverterFactory = OcelotGateway.Hateoas.Serialization.HateoasJsonConverterFactory;

namespace OcelotGateway.Hateoas;

internal static class Extensions
{
    internal static IServiceCollection AddHateoas(this IServiceCollection services)
    {
        services.AddSingleton<JsonConverterFactory, HateoasJsonConverterFactory>();
        services.AddSingleton<IHateoasLinksBuilderFactory, HateoasLinksBuilderFactory>();
        services.ConfigureOptions<ConfigureHateoasMvcOptions>();
        services.AddSingleton<IRouteCollection>(serviceProvider =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var routeConfigs = configuration.GetSection("Routes").Get<List<OcelotRouteConfiguration>>();
            return new RouteCollection(routeConfigs);
        });

        services.Scan(scan => scan
            .FromEntryAssembly()
            .AddClasses(c => c.AssignableTo(typeof(IHateoasLinksConfiguration)), false)
            .AsImplementedInterfaces()
            .WithSingletonLifetime());

        return services;
    }
}