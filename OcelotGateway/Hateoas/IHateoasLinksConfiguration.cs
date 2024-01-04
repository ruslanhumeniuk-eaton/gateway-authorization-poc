namespace OcelotGateway.Hateoas;

/// <summary>
///     Interface for configuring <see cref="HateoasLinksBuilder" />.
/// </summary>
public interface IHateoasLinksConfiguration
{
    public NestedConfigurationsDictionary? NestedObjectsConfigurations { get; set; }
    /// <summary>
    ///     Configure the <see cref="HateoasLinksBuilder" />.
    /// </summary>
    /// <param name="linksBuilder"><see cref="HateoasLinksBuilder" />.</param>
    public void Configure(HateoasLinksBuilder linksBuilder);
}