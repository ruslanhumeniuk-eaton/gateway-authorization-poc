using Newtonsoft.Json.Serialization;

namespace Shared.Serialization;

/// <summary>
///     JSON contract resolver.
/// </summary>
public class JsonContractResolver : DefaultContractResolver
{
    private readonly IEnumerable<IJsonConverterFactory> _jsonConverterFactories;

    /// <summary>
    ///     Initializes a new instance of the <see cref="JsonContractResolver" /> class.
    /// </summary>
    /// <param name="jsonConverterFactories">Collection of JSON converter factories.</param>
    public JsonContractResolver(IEnumerable<IJsonConverterFactory> jsonConverterFactories)
    {
        _jsonConverterFactories = jsonConverterFactories;
        NamingStrategy = new CamelCaseNamingStrategy();
    }

    /// <inheritdoc />
    protected override JsonContract CreateContract(Type objectType)
    {
        JsonContract contract = base.CreateContract(objectType);

        IJsonConverterFactory factory = _jsonConverterFactories.FirstOrDefault(f => f.CanConvert(objectType));
        if (factory != null)
        {
            contract.Converter = factory.CreateConverter(objectType, NamingStrategy);
        }

        return contract;
    }
}