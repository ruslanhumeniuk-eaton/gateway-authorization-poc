using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace OcelotGateway.Hateoas.Serialization;

/// <summary>
///     <see cref="MvcOptions" /> configuration class.
/// </summary>
internal class ConfigureHateoasMvcOptions : IConfigureOptions<MvcOptions>
{
    /// <summary>
    ///     Configure MVC options.
    ///     Add Hypertext Application Language content type.
    /// </summary>
    /// <param name="options">MVC options.</param>
    public void Configure(MvcOptions options)
    {
        options.Filters.Add(new ProducesAttribute("application/hal+json"));
    }
}