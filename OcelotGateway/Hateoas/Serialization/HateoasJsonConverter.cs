using Contracts.Application;
using System.Text.Json;
using System.Text.Json.Serialization;
using Humanizer;

namespace OcelotGateway.Hateoas.Serialization;

public class HateoasJsonConverter<TConfiguration> : JsonConverter<JsonElement>
    where TConfiguration : class, IHateoasLinksConfiguration
{
    private const string LinksPropertyName = "_links";
    private const string EmbeddedPropertyName = "_embedded";
    private readonly IHateoasLinksBuilderFactory _hateoasLinksBuilderFactory;

    public HateoasJsonConverter(IHateoasLinksBuilderFactory hateoasLinksBuilderFactory)
    {
        _hateoasLinksBuilderFactory = hateoasLinksBuilderFactory;
    }

    public override void Write(Utf8JsonWriter writer, JsonElement value, JsonSerializerOptions options)
    {
        ProcessJsonElement(writer, value, options, typeof(TConfiguration));
    }

    public override JsonElement Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    private void ProcessJsonElement(Utf8JsonWriter writer, JsonElement value, JsonSerializerOptions options,
        Type? configurationType = null)
    {
        switch (value.ValueKind)
        {
            case JsonValueKind.Object:
                ProcessObject(writer, value.EnumerateObject(), options, configurationType);
                break;
            case JsonValueKind.Array:
                ProcessArray(writer, value.EnumerateArray(), options, configurationType);
                break;
            case JsonValueKind.Undefined:
            case JsonValueKind.String:
            case JsonValueKind.Number:
            case JsonValueKind.True:
            case JsonValueKind.False:
            case JsonValueKind.Null:
                writer.WriteRawValue(value.GetRawText());
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void ProcessObject(Utf8JsonWriter writer, JsonElement.ObjectEnumerator element,
        JsonSerializerOptions options, Type configurationType)
    {
        writer.WriteStartObject();
        var embeddedProperties = new Dictionary<string, object>();

        foreach (var property in element)
        {
            if (typeof(ResourceCollectionDto).GetProperties().Any(x =>
                    string.Equals(property.Name, x.Name, StringComparison.InvariantCultureIgnoreCase)))
            {
                embeddedProperties.Add(property.Name.Camelize(), property.Value);
            }
            else
            {
                ProcessProperty(writer, property, options, configurationType);
            }
        }

        if (embeddedProperties.Any())
        {
            writer.WritePropertyName(EmbeddedPropertyName);
            JsonSerializer.Serialize(writer, embeddedProperties, options);
        }

        if (configurationType != null)
        {
            var linksBuilder = _hateoasLinksBuilderFactory.ConstructLinksBuilder(configurationType);
            var links = linksBuilder.BuildLinksAsync(element).GetAwaiter().GetResult();

            if (links.Any())
            {
                writer.WritePropertyName(LinksPropertyName);
                JsonSerializer.Serialize(writer, links, options);
            }
        }

        writer.WriteEndObject();
    }

    private void ProcessProperty(Utf8JsonWriter writer, JsonProperty property,
        JsonSerializerOptions options, Type? configurationType)
    {
        writer.WritePropertyName(property.Name);

        var configuration = configurationType is not null
            ? (IHateoasLinksConfiguration)Activator.CreateInstance(configurationType)
            : null;
        var nestedConfigs = configuration?.NestedObjectsConfigurations?.ConfigurationsDictionary;
        var nestedConfigsValid = nestedConfigs is not null && nestedConfigs.Any();

        if (nestedConfigsValid && nestedConfigs.ContainsKey(property.Name.ToLower()))
        {
            ProcessJsonElement(writer, property.Value, options, nestedConfigs[property.Name.ToLower()]);
        }
        else
        {
            ProcessJsonElement(writer, property.Value, options);
        }
    }

    private void ProcessArray(Utf8JsonWriter writer, JsonElement.ArrayEnumerator array, JsonSerializerOptions options,
        Type configurationType)
    {
        writer.WriteStartArray();

        foreach (var value in array)
        {
            ProcessJsonElement(writer, value, options, configurationType);
        }

        writer.WriteEndArray();
    }
}