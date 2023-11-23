namespace OcelotGateway.Hateoas;

/// <summary>
///     Interface for retrieving a route by its name.
/// </summary>
public interface IRouteCollection
{
    /// <summary>
    ///     Try to retrieve a route by its name.
    /// </summary>
    /// <param name="routeName">Route name.</param>
    /// <param name="route">Route. Can be null.</param>
    /// <returns>True if route is found.</returns>
    internal bool TryGetRoute(string routeName, out Route route);
}