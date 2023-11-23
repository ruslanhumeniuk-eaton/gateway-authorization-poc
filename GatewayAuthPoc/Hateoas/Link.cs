namespace OcelotGateway.Hateoas;

/// <summary>
///     Utility class for link representation.
/// </summary>
internal class Link
{
    /// <summary>
    ///     Gets the link.
    /// </summary>
    public string Href { get; }

    /// <summary>
    ///     Gets the HTTP method.
    /// </summary>
    public string Method { get; }

    /// <summary>
    ///     Gets a value indicating whether the link is templated or not.
    /// </summary>
    public bool Templated => Href.IndexOf('{') >= 0;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Link" /> class.
    /// </summary>
    /// <param name="href">Link value.</param>
    /// <param name="method">HTTP method.</param>
    public Link(string href, string method)
    {
        Href = href;
        Method = method;
    }
}