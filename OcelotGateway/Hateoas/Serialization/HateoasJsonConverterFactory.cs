using System.Text.Json;
using System.Text.Json.Serialization;

namespace OcelotGateway.Hateoas.Serialization;

public class HateoasJsonConverterFactory : JsonConverterFactory
{
    private readonly Type _jsonConverterType = typeof(HateoasJsonConverter<>);
    private readonly IHateoasLinksBuilderFactory _hateoasLinksBuilderFactory;

    public HateoasJsonConverterFactory(IHateoasLinksBuilderFactory hateoasLinksBuilderFactory)
    {
        _hateoasLinksBuilderFactory = hateoasLinksBuilderFactory;
    }

    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(IHateoasLinksConfiguration).IsAssignableFrom(typeToConvert);
    }

    public override JsonConverter? CreateConverter(Type configurationType, JsonSerializerOptions options)
    {
        var converterType = _jsonConverterType.MakeGenericType(configurationType);

        return (JsonConverter)Activator.CreateInstance(converterType, _hateoasLinksBuilderFactory);
    }
}