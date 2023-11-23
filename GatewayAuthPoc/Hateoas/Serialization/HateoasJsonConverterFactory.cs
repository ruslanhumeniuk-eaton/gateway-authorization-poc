using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Shared.Serialization;

namespace OcelotGateway.Hateoas.Serialization;

/// <summary>
///     HATEOAS JSON converter factory.
/// </summary>
public class HateoasJsonConverterFactory : IJsonConverterFactory
{
    private readonly Type _builderType = typeof(HateoasLinksBuilder<>);
    private readonly Type _configurationType = typeof(IHateoasLinksConfiguration<>);
    private readonly Type _jsonConverterType = typeof(HateoasJsonConverter<,>);
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    ///     Initializes a new instance of the <see cref="HateoasJsonConverterFactory" /> class.
    /// </summary>
    /// <param name="serviceProvider">Service provider.</param>
    public HateoasJsonConverterFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public bool CanConvert(Type objectType) => GetConfiguration(objectType) != null;

    /// <inheritdoc />
    public JsonConverter CreateConverter(Type objectType, NamingStrategy namingStrategy)
    {
        object configuration = GetConfiguration(objectType);

        Type configuredType = GetConfiguredType(configuration);

        ObjectFactory builderFactory = ActivatorUtilities.CreateFactory(_builderType.MakeGenericType(configuredType), Type.EmptyTypes);
        object builder = builderFactory(_serviceProvider, null);

        Type converterType = _jsonConverterType.MakeGenericType(objectType, configuredType);

        return (JsonConverter)Activator.CreateInstance(converterType, builder, namingStrategy);
    }

    /// <summary>
    ///     Get type configured in the given configuration object.
    /// </summary>
    /// <param name="configuration">Configuration object.</param>
    /// <returns>Configured type.</returns>
    private Type GetConfiguredType(object configuration)
    {
        Type t = configuration.GetType();

        return t.GetInterface(_configurationType.Name)?.GenericTypeArguments[0];
    }

    /// <summary>
    ///     Get HATEOAS links configuration for the given type or parent type. Null if not found.
    /// </summary>
    /// <param name="typeToConvert">Type of object to configure links.</param>
    /// <returns>Configuration. Can be null.</returns>
    private object GetConfiguration(Type typeToConvert)
    {
        while (true)
        {
            if (typeToConvert is null || !typeToConvert.IsClass)
            {
                return null;
            }

            object configuration = _serviceProvider.GetService(_configurationType.MakeGenericType(typeToConvert));
            if (configuration != null)
            {
                return configuration;
            }

            typeToConvert = typeToConvert.BaseType;
        }
    }
}