using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Shared.Serialization;

namespace Shared.ResourceIdConfiguration;

/// <summary>
///     <see cref="ResourceIdConverter{T}" /> factory. Allows to instantiate the generic converter associated to the
///     resource id type.
/// </summary>
public class ResourceIdJsonConverterFactory : IJsonConverterFactory
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ResourceIdJsonConverterFactory" /> class.
    /// </summary>
    /// <param name="serviceProvider">Service provider.</param>
    public ResourceIdJsonConverterFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public bool CanConvert(Type objectType) => objectType.IsResourceIdType();

    /// <inheritdoc />
    public JsonConverter CreateConverter(Type objectType, NamingStrategy namingStrategy)
    {
        Type keyType = objectType.GenericTypeArguments[0];
        Type converterType = typeof(ResourceIdConverter<>).MakeGenericType(keyType);

        return (JsonConverter)ActivatorUtilities.CreateInstance(_serviceProvider, converterType);
    }
}