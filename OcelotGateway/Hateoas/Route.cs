namespace OcelotGateway.Hateoas;

/// <summary>
///     HTTP route.
/// </summary>
public class Route
{
    /// <summary>
    ///     Gets route name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Gets link template.
    /// </summary>
    public LinkTemplate Template { get; }

    /// <summary>
    ///     Gets HTTP method.
    /// </summary>
    public string HttpMethod { get; }

    /// <summary>
    ///     Gets list of permissions.
    /// </summary>
    public IReadOnlyCollection<Permission> Permissions { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Route" /> class.
    /// </summary>
    /// <param name="name">Route name.</param>
    /// <param name="template">Link template.</param>
    /// <param name="httpMethod">HTTP method.</param>
    /// <param name="permissions">List of permissions.</param>
    public Route(string name, LinkTemplate template, string httpMethod, IEnumerable<Permission> permissions)
    {
        Name = name;
        Template = template;
        HttpMethod = httpMethod;
        Permissions = permissions.ToList();
    }
}