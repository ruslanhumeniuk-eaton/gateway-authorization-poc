using System.Text.Json;
using Contracts.Application;
using Humanizer;
using Shared;
using Shared.Encryption;

namespace OcelotGateway.Hateoas;

/// <summary>
///     HATEOAS links builder.
/// </summary>
public class HateoasLinksBuilder
{
    private const string Page = "page";
    private readonly Dictionary<string, RouteBuilder> _additionalRoutes;
    private readonly IEncryptor _encryptor;
    private readonly IRouteCollection _routeCollection;
    private readonly IServiceProvider _serviceProvider;
    private RouteBuilder _selfRouteBuilder;

    /// <summary>
    ///     Initializes a new instance of the <see cref="HateoasLinksBuilder" /> class.
    /// </summary>
    /// <param name="linksConfiguration">HATEOAS links configuration.</param>
    /// <param name="encryptor">Encryptor.</param>
    /// <param name="routeCollection">Route collection.</param>
    /// <param name="serviceProvider">Service provider.</param>
    public HateoasLinksBuilder(IHateoasLinksConfiguration linksConfiguration, IEncryptor encryptor,
        IRouteCollection routeCollection,
        IServiceProvider serviceProvider)
    {
        _encryptor = encryptor;
        _routeCollection = routeCollection;
        _serviceProvider = serviceProvider;
        _additionalRoutes = new Dictionary<string, RouteBuilder>();
        linksConfiguration.Configure(this);
    }

    /// <summary>
    ///     Build links dictionary for the given object.
    /// </summary>
    /// <param name="o">Object to configure.</param>
    /// <returns>Dictionary of links. Can be null.</returns>
    public Task<IDictionary<string, Link>> BuildLinksAsync(JsonElement.ObjectEnumerator o)
    {
        return GetLinksAsync(o);
    }

    /// <summary>
    ///     Add link to self.
    /// </summary>
    /// <param name="routeName">Route name.</param>
    /// <returns>Route builder.</returns>
    public RouteBuilder SelfLink(string routeName)
    {
        _selfRouteBuilder = new RouteBuilder(routeName);
        return _selfRouteBuilder;
    }

    /// <summary>
    ///     Add link to self.
    /// </summary>
    /// <param name="routeName">Route name.</param>
    /// <param name="condition">Condition to satisfy for adding the link.</param>
    /// <returns>Route builder.</returns>
    public RouteBuilder SelfLink(string routeName, Func<JsonElement.ObjectEnumerator, bool> condition)
    {
        _selfRouteBuilder = new RouteBuilder(routeName, condition);
        return _selfRouteBuilder;
    }

    /// <summary>
    ///     Add link of the given name for the given route.
    /// </summary>
    /// <param name="link">Link name.</param>
    /// <param name="routeName">Route name.</param>
    /// <returns>Route builder.</returns>
    public RouteBuilder AddLink(string link, string routeName)
    {
        var routeBuilder = new RouteBuilder(routeName);
        _additionalRoutes.Add(link, routeBuilder);
        return routeBuilder;
    }

    /// <summary>
    ///     Add link of the given name for the given route. If condition is satisfied.
    /// </summary>
    /// <param name="link">Link name.</param>
    /// <param name="routeName">Route name.</param>
    /// <param name="condition">Condition to satisfy for adding the link.</param>
    /// <returns>Route builder.</returns>
    public RouteBuilder AddLink(string link, string routeName, Func<JsonElement.ObjectEnumerator, bool> condition)
    {
        var routeBuilder = new RouteBuilder(routeName, condition);
        _additionalRoutes.Add(link, routeBuilder);
        return routeBuilder;
    }

    private static async Task<bool> IsAuthorized(
        IEnumerable<Permission> permissions,
        IList<KeyValuePair<string, object>> rawPlaceholders,
        IAuthorizationService authorizationService)
    {
        foreach (Permission routePermission in permissions)
        {
            KeyValuePair<string, object> organizationIdPair = rawPlaceholders
                .FirstOrDefault(p => p.Key == routePermission.OrganizationIdKey && p.Value is ResourceId<Guid>);

            if ((object)organizationIdPair.Value is not ResourceId<Guid> organizationId)
            {
                // return true so the link is generated with the organization id template.
                return true;
            }

            bool isAuthorized =
                await authorizationService.IsAuthorizedAsync(routePermission.PermissionKey,
                    organizationId.Value.ToString());
            if (!isAuthorized)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    ///     Append collection:first link.
    /// </summary>
    /// <param name="links">Links dictionary where to append.</param>
    /// <param name="route">Route.</param>
    /// <param name="placeHolders">List of placeholders.</param>
    private static void AddCollectionFirstLink(IDictionary<string, Link> links, Route route,
        IEnumerable<KeyValuePair<string, string>> placeHolders)
    {
        var collectionFirstPlaceHolders = new List<KeyValuePair<string, string>> { new(Page, "1") };
        collectionFirstPlaceHolders.AddRange(placeHolders.Where(kvp => kvp.Key != Page));

        links.Add($"{HateoasLinkConsts.Collection}:first",
            new Link(route.Template.Format(collectionFirstPlaceHolders), route.HttpMethod));
    }

    /// <summary>
    ///     Append collection:last link.
    /// </summary>
    /// <param name="links">Links dictionary where to append.</param>
    /// <param name="route">Route.</param>
    /// <param name="placeHolders">List of placeholders.</param>
    /// <param name="resourceCollectionDto">Collection.</param>
    private static void AddCollectionLastLink(IDictionary<string, Link> links, Route route,
        IEnumerable<KeyValuePair<string, string>> placeHolders,
        ResourceCollectionDto resourceCollectionDto)
    {
        int? pageNumber = GetLastPageNumber(resourceCollectionDto.Total, resourceCollectionDto.PageSize);
        if (!pageNumber.HasValue)
        {
            return;
        }

        var collectionFirstPlaceHolders = new List<KeyValuePair<string, string>>
            { new(Page, pageNumber.Value.ToString()) };
        collectionFirstPlaceHolders.AddRange(placeHolders.Where(kvp => kvp.Key != Page));

        links.Add($"{HateoasLinkConsts.Collection}:last",
            new Link(route.Template.Format(collectionFirstPlaceHolders), route.HttpMethod));
    }

    /// <summary>
    ///     Append collection:previous link.
    /// </summary>
    /// <param name="links">Links dictionary where to append.</param>
    /// <param name="route">Route.</param>
    /// <param name="placeHolders">List of placeholders.</param>
    /// <param name="resourceCollectionDto">Collection.</param>
    private static void AddCollectionPreviousLink(IDictionary<string, Link> links, Route route,
        IEnumerable<KeyValuePair<string, string>> placeHolders,
        ResourceCollectionDto resourceCollectionDto)
    {
        if (resourceCollectionDto.Page is null or 1)
        {
            return;
        }

        int pageNumber = resourceCollectionDto.Page.Value - 1;
        var collectionFirstPlaceHolders = new List<KeyValuePair<string, string>> { new(Page, pageNumber.ToString()) };
        collectionFirstPlaceHolders.AddRange(placeHolders.Where(kvp => kvp.Key != Page));

        links.Add($"{HateoasLinkConsts.Collection}:previous",
            new Link(route.Template.Format(collectionFirstPlaceHolders), route.HttpMethod));
    }

    /// <summary>
    ///     Get collection last page number.
    /// </summary>
    /// <param name="total">Total items count.</param>
    /// <param name="pageSize">Page size.</param>
    /// <returns>Last page number. Can be null.</returns>
    private static int? GetLastPageNumber(int total, int? pageSize) => (total + pageSize - 1) / pageSize;

    /// <summary>
    ///     Append collection:page link.
    /// </summary>
    /// <param name="links">Links dictionary where to append.</param>
    /// <param name="route">Route.</param>
    /// <param name="placeHolders">List of placeholders.</param>
    private static void AddCollectionPageLink(IDictionary<string, Link> links, Route route,
        IEnumerable<KeyValuePair<string, string>> placeHolders)
    {
        var collectionFirstPlaceHolders = new List<KeyValuePair<string, string>>();
        collectionFirstPlaceHolders.AddRange(placeHolders.Where(kvp => kvp.Key != Page));
        links.Add($"{HateoasLinkConsts.Collection}:page",
            new Link(route.Template.Format(collectionFirstPlaceHolders), route.HttpMethod));
    }

    /// <summary>
    ///     Append collection:sort link.
    /// </summary>
    /// <param name="links">Links dictionary where to append.</param>
    /// <param name="route">Route.</param>
    /// <param name="placeHolders">List of placeholders.</param>
    private static void AddCollectionSortLink(IDictionary<string, Link> links, Route route,
        IEnumerable<KeyValuePair<string, string>> placeHolders)
    {
        const string sortBy = "sortBy";
        const string sortOrder = "sortOrder";
        var collectionFirstPlaceHolders = new List<KeyValuePair<string, string>> { new(Page, "1") };
        collectionFirstPlaceHolders.AddRange(placeHolders.Where(kvp =>
            kvp.Key != Page && kvp.Key != sortBy && kvp.Key != sortOrder));

        links.Add($"{HateoasLinkConsts.Collection}:sort",
            new Link(route.Template.Format(collectionFirstPlaceHolders), route.HttpMethod));
    }

    /// <summary>
    ///     Append collection:filter link.
    /// </summary>
    /// <param name="links">Links dictionary where to append.</param>
    /// <param name="route">Route.</param>
    /// <param name="placeHolders">List of placeholders.</param>
    private static void AddCollectionFilterLink(IDictionary<string, Link> links, Route route,
        IEnumerable<KeyValuePair<string, string>> placeHolders)
    {
        const string filterBy = "filterBy";
        const string filterCriteria = "filterCriteria";
        var collectionFirstPlaceHolders = new List<KeyValuePair<string, string>> { new(Page, "1") };
        collectionFirstPlaceHolders.AddRange(placeHolders.Where(kvp =>
            kvp.Key != Page && kvp.Key != filterBy && kvp.Key != filterCriteria));

        links.Add($"{HateoasLinkConsts.Collection}:filter",
            new Link(route.Template.Format(collectionFirstPlaceHolders), route.HttpMethod));
    }

    /// <summary>
    ///     Get unencrypted place holders for the given route builder and object.
    /// </summary>
    /// <param name="routeBuilder">Route builder.</param>
    /// <param name="o">Object.</param>
    /// <returns>List of placeholders.</returns>
    private static IList<KeyValuePair<string, object>> GetRawPlaceHolders(RouteBuilder routeBuilder,
        JsonElement.ObjectEnumerator o)
    {
        var properties = new List<KeyValuePair<string, object>>();
        properties.AddRange(routeBuilder.GetPlaceHolders(o));
        properties.AddRange(GetProperties(o));

        return properties;
    }

    /// <summary>
    ///     Get collection of property name and value of the given object.
    /// </summary>
    /// <param name="o">Object (DTO).</param>
    /// <returns>Collection of key value pair.</returns>
    private static IEnumerable<KeyValuePair<string, object>> GetProperties(JsonElement.ObjectEnumerator o)
    {
        return o.Select(x => new KeyValuePair<string, object>(x.Name.Camelize(), x.Value.GetRawText()));
    }

    /// <summary>
    ///     Append collection:next link.
    /// </summary>
    /// <param name="links">Links dictionary where to append.</param>
    /// <param name="route">Route.</param>
    /// <param name="placeHolders">List of placeholders.</param>
    /// <param name="resourceCollectionDto">Collection.</param>
    private static void AddCollectionNextLink(IDictionary<string, Link> links, Route route,
        IEnumerable<KeyValuePair<string, string>> placeHolders,
        ResourceCollectionDto resourceCollectionDto)
    {
        int? lastPageNumber = GetLastPageNumber(resourceCollectionDto.Total, resourceCollectionDto.PageSize);
        if (!resourceCollectionDto.Page.HasValue || !lastPageNumber.HasValue ||
            resourceCollectionDto.Page.Value == lastPageNumber.Value)
        {
            return;
        }

        int pageNumber = resourceCollectionDto.Page.Value + 1;

        var collectionFirstPlaceHolders = new List<KeyValuePair<string, string>> { new(Page, pageNumber.ToString()) };
        collectionFirstPlaceHolders.AddRange(placeHolders.Where(kvp => kvp.Key != Page));

        links.Add($"{HateoasLinkConsts.Collection}:next",
            new Link(route.Template.Format(collectionFirstPlaceHolders), route.HttpMethod));
    }

    /// <summary>
    ///     Get links dictionary for the given object.
    /// </summary>
    /// <param name="o">Object.</param>
    /// <returns>Dictionary of links.</returns>
    private async Task<IDictionary<string, Link>> GetLinksAsync(JsonElement.ObjectEnumerator o)
    {
        var links = new Dictionary<string, Link>();

        var authorizationService = _serviceProvider.GetRequiredService<IAuthorizationService>();

        await AddSelfLinks(links, o, authorizationService);

        await AddAdditionalLinks(links, o, authorizationService);

        return links;
    }

    private async Task AddSelfLinks(IDictionary<string, Link> links, JsonElement.ObjectEnumerator o,
        IAuthorizationService authorizationService)
    {
        if (_selfRouteBuilder == null || !_selfRouteBuilder.CanCreate(o))
        {
            return;
        }

        // resource collection must contain a property 'Total' ('Page' and 'PageSize' are optional)

        if (o.Any(x => x.NameEquals(nameof(ResourceCollectionDto.Total))))
        {
            AddCollectionLinks(links, o);
        }
        else
        {
            await AddLink(links, o, HateoasLinkConsts.Self, _selfRouteBuilder, authorizationService);
        }
    }

    private async Task AddAdditionalLinks(IDictionary<string, Link> links, JsonElement.ObjectEnumerator o,
        IAuthorizationService authorizationService)
    {
        foreach ((string linkName, RouteBuilder routeBuilder) in _additionalRoutes.Where(r => r.Value.CanCreate(o)))
        {
            await AddLink(links, o, linkName, routeBuilder, authorizationService);
        }
    }

    private async Task AddLink(
        IDictionary<string, Link> links,
        JsonElement.ObjectEnumerator sourceObject,
        string linkName,
        RouteBuilder routeBuilder,
        IAuthorizationService authorizationService)
    {
        if (!_routeCollection.TryGetRoute(routeBuilder.RouteName, out Route route))
        {
            return;
        }

        IList<KeyValuePair<string, object>> rawPlaceHolders = GetRawPlaceHolders(routeBuilder, sourceObject);

        if (!await IsAuthorized(route.Permissions.ToList(), rawPlaceHolders, authorizationService))
        {
            return;
        }

        IList<KeyValuePair<string, string>> placeHolders = EncryptPlaceHolders(rawPlaceHolders);

        links.Add(linkName, new Link(route.Template.Format(placeHolders), route.HttpMethod));
    }

    /// <summary>
    ///     Append collection links to the given dictionary for object of type <see cref="ResourceCollectionDto" />.
    /// </summary>
    /// <param name="links">Links dictionary where to append.</param>
    /// <param name="resourceCollection">Collection.</param>
    private void AddCollectionLinks(IDictionary<string, Link> links, JsonElement.ObjectEnumerator resourceCollection)
    {
        if (!_routeCollection.TryGetRoute(_selfRouteBuilder.RouteName, out Route route))
        {
            return;
        }

        IList<KeyValuePair<string, string>> placeHolders =
            GetEncryptedPlaceHolders(_selfRouteBuilder, resourceCollection);

        AddCollectionFirstLink(links, route, placeHolders);
        AddCollectionLastLink(links, route, placeHolders, GetResourceCollectionDtoObject(resourceCollection));
        AddCollectionPreviousLink(links, route, placeHolders, GetResourceCollectionDtoObject(resourceCollection));
        AddCollectionNextLink(links, route, placeHolders, GetResourceCollectionDtoObject(resourceCollection));
        AddCollectionPageLink(links, route, placeHolders);
        AddCollectionSortLink(links, route, placeHolders);
        AddCollectionFilterLink(links, route, placeHolders);
    }

    /// <summary>
    ///     Get encrypted place holders for the given route builder and object.
    /// </summary>
    /// <param name="routeBuilder">Route builder.</param>
    /// <param name="o">Object.</param>
    /// <returns>List of encrypted place holders.</returns>
    private IList<KeyValuePair<string, string>> GetEncryptedPlaceHolders(RouteBuilder routeBuilder,
        JsonElement.ObjectEnumerator o) =>
        EncryptPlaceHolders(GetRawPlaceHolders(routeBuilder, o));

    /// <summary>
    ///     Encrypt all resource id place holders.
    /// </summary>
    /// <param name="placeHolders">List of place holders.</param>
    /// <returns>List of encrypted place holders.</returns>
    private IList<KeyValuePair<string, string>> EncryptPlaceHolders(
        IEnumerable<KeyValuePair<string, object>> placeHolders)
    {
        var encryptedPlaceHolders = new List<KeyValuePair<string, string>>();

        foreach ((string key, object value) in placeHolders)
        {
            if (value is null)
            {
                continue;
            }

            // here was the ResourceId check but it was replaced with simple Guid TryParse as we have dynamic types from now and ResourceId type is not simply checkable here
            string stringValue = Guid.TryParse(value.ToString().Trim('"'), out _)
                ? _encryptor.Encrypt(value)
                : value.ToString();

            encryptedPlaceHolders.Add(new KeyValuePair<string, string>(key, stringValue));
        }

        return encryptedPlaceHolders;
    }

    private ResourceCollectionDto GetResourceCollectionDtoObject(JsonElement.ObjectEnumerator o)
    {
        int? page = null;
        int? pageSize = null;
        int total = 0;

        foreach (var property in o)
        {
            if (!TryGetInt(property.Value, out var converted))
            {
                continue;
            }

            if (string.Equals(property.Name, nameof(ResourceCollectionDto.Page),
                    StringComparison.InvariantCultureIgnoreCase))
            {
                page = converted;
            }
            else if (string.Equals(property.Name, nameof(ResourceCollectionDto.PageSize),
                         StringComparison.InvariantCultureIgnoreCase))
            {
                pageSize = converted;
            }
            else if (string.Equals(property.Name, nameof(ResourceCollectionDto.Total),
                         StringComparison.InvariantCultureIgnoreCase))
            {
                total = converted;
            }
        }

        return new ResourceCollectionDtoStub { Page = page, PageSize = pageSize, Total = total };
    }

    private bool TryGetInt(JsonElement value, out int converted)
    {
        converted = 0;

        return value.ValueKind switch
        {
            JsonValueKind.Number => value.TryGetInt32(out converted),
            _ => false
        };
    }

    private class ResourceCollectionDtoStub : ResourceCollectionDto
    {
    }
}