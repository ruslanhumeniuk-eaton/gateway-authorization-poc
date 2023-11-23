namespace OcelotGateway.Hateoas;

/// <summary>
///     Interface for configuring <see cref="HateoasLinksBuilder{T}" />.
/// </summary>
/// <typeparam name="T">Type of object to configure to which configure HATEOAS.</typeparam>
public interface IHateoasLinksConfiguration<T>
    where T : class
{
    /// <summary>
    ///     Configure the <see cref="HateoasLinksBuilder{T}" />.
    /// </summary>
    /// <param name="linksBuilder"><see cref="HateoasLinksBuilder{T}" />.</param>
    internal void Configure(HateoasLinksBuilder<T> linksBuilder);
}