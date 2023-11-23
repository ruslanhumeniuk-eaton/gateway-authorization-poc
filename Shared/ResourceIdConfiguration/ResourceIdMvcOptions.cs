using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Shared.ResourceIdConfiguration;

/// <summary>
///     <see cref="MvcOptions" /> configuration class.
/// </summary>
public class ResourceIdMvcOptions: IConfigureOptions<MvcOptions>
{
    /// <summary>
    ///     Configure MVC options.
    ///     Add ResourceId model binder.
    /// </summary>
    /// <param name="options">MVC options.</param>
    public void Configure(MvcOptions options)
    {
        options.ModelBinderProviders.Insert(0, new ResourceIdModelBinderProvider());
    }
}