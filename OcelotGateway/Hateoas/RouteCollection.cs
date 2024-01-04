using OcelotGateway.Ocelot;

namespace OcelotGateway.Hateoas;

/// <summary>
///     Implementation of the route collection.
/// </summary>
internal class RouteCollection : IRouteCollection
{
    private readonly IDictionary<string, Route> _routes;
    public const string OrganizationIdRouteKey = "organizationId";

    /// <summary>
    ///     Initializes a new instance of the <see cref="RouteCollection" /> class.
    /// </summary>
    /// <param name="ocelotRoutes">Collection of Ocelot routes.</param>
    public RouteCollection(IEnumerable<OcelotRouteConfiguration> ocelotRoutes)
    {
        _routes = GetRoutes(ocelotRoutes).ToDictionary(r => r.Name, r => r);
    }

    /// <inheritdoc />
    public bool TryGetRoute(string routeName, out Route route) => _routes.TryGetValue(routeName, out route);

    /// <summary>
    ///     Get all named routes.
    /// </summary>
    /// <param name="ocelotRoutes">List of Action descriptors.</param>
    /// <returns>List of routes.</returns>
    private IEnumerable<Route> GetRoutes(IEnumerable<OcelotRouteConfiguration> ocelotRoutes) =>
        ocelotRoutes.Select(GetRoute).Where(r => !string.IsNullOrWhiteSpace(r.Name));

    /// <summary>
    ///     Get route from action descriptor.
    /// </summary>
    /// <param name="ocelotRouteConfiguration">Ocelot route configuration.</param>
    /// <returns>Route.</returns>
    private Route GetRoute(OcelotRouteConfiguration ocelotRouteConfiguration)
    {
        var permissions = new List<Permission>();

        if (ocelotRouteConfiguration.PermissionKeys is not null)
        {
            permissions.AddRange(
                ocelotRouteConfiguration.PermissionKeys.Select(x => new Permission(x, OrganizationIdRouteKey)));
        }

        return new Route(
            ocelotRouteConfiguration.Name,
            new LinkTemplate(ocelotRouteConfiguration.UpstreamPathTemplate),
            ocelotRouteConfiguration.UpstreamHttpMethod.First(),
            permissions);
    }
}