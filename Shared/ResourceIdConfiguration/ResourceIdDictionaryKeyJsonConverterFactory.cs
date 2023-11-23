using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Shared.Serialization;

namespace Shared.ResourceIdConfiguration;

/// <summary>
///     <see cref="ResourceIdDictionaryKeyConverter{TKey, TValue}" /> factory.
/// </summary>
public class ResourceIdDictionaryKeyJsonConverterFactory : IJsonConverterFactory
{
    private const string DictionaryInterfaceTypeName = "IDictionary";

    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ResourceIdDictionaryKeyJsonConverterFactory" /> class.
    /// </summary>
    /// <param name="serviceProvider">Service provider.</param>
    public ResourceIdDictionaryKeyJsonConverterFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public bool CanConvert(Type objectType) => objectType.IsGenericType
                                               && objectType.GenericTypeArguments[0].IsResourceIdType()
                                               && objectType.GetInterface(DictionaryInterfaceTypeName) != null;

    /// <inheritdoc />
    public JsonConverter CreateConverter(Type objectType, NamingStrategy namingStrategy)
    {
        Type resourceIdKeyType = objectType.GenericTypeArguments[0].GenericTypeArguments[0];
        Type valueType = objectType.GenericTypeArguments[1];
        Type converterType = typeof(ResourceIdDictionaryKeyConverter<,>).MakeGenericType(resourceIdKeyType, valueType);

        return (JsonConverter)ActivatorUtilities.CreateInstance(_serviceProvider, converterType);
    }
}