using System.Collections;
using System.Reflection;
using Contracts.Application;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace OcelotGateway.Hateoas.Serialization;

/// <summary>
///     HATEOAS JSON Converter.
/// </summary>
/// <typeparam name="TConverter">DTO type to convert.</typeparam>
/// <typeparam name="TConfiguration">DTO configuration type.</typeparam>
internal class HateoasJsonConverter<TConverter, TConfiguration> : JsonConverter<TConverter>
    where TConverter : class, TConfiguration
    where TConfiguration : class
{
    private const string LinksPropertyName = "_links";
    private const string EmbeddedPropertyName = "_embedded";
    private readonly HateoasLinksBuilder<TConfiguration> _linksBuilder;
    private readonly NamingStrategy _namingStrategy;

    /// <inheritdoc />
    public override bool CanRead => false;

    /// <summary>
    ///     Initializes a new instance of the <see cref="HateoasJsonConverter{TConverter, TConfiguration}" /> class.
    /// </summary>
    /// <param name="linksBuilder">HATEOAS links builder for the given type.</param>
    /// <param name="namingStrategy">Naming strategy.</param>
    public HateoasJsonConverter(HateoasLinksBuilder<TConfiguration> linksBuilder, NamingStrategy namingStrategy)
    {
        _linksBuilder = linksBuilder;
        _namingStrategy = namingStrategy;
    }

    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, TConverter value, JsonSerializer serializer)
    {
        var jo = new JObject();

        WriteObjectProperties(value, jo, serializer);

        IDictionary<string, Link> links = _linksBuilder.BuildLinksAsync(value).GetAwaiter().GetResult();
        if (links.Any())
        {
            jo.Add(new JProperty(LinksPropertyName, JToken.FromObject(links, serializer)));
        }

        jo.WriteTo(writer);
    }

    /// <inheritdoc />
    public override TConverter ReadJson(JsonReader reader, Type objectType, TConverter existingValue, bool hasExistingValue, JsonSerializer serializer) =>
        throw new NotImplementedException("This JSON converter can only write JSON.");

    /// <summary>
    ///     Write object properties to the JSON object.
    ///     If it's a collection, is embedded in "_embedded" property.
    /// </summary>
    /// <param name="value">Object to convert.</param>
    /// <param name="jo">Json object to populate.</param>
    /// <param name="serializer">JSON serializer.</param>
    private void WriteObjectProperties(TConverter value, JObject jo, JsonSerializer serializer)
    {
        Type valueType = typeof(TConverter);
        IDictionary<string, object> embeddedProperties = new Dictionary<string, object>();

        foreach (PropertyInfo property in valueType.GetProperties().Where(p => p.GetCustomAttribute<JsonIgnoreAttribute>() is null))
        {
            object propertyValue = property.GetValue(value);
            bool isPropertyValueNull = propertyValue is null;
            if (isPropertyValueNull && serializer.NullValueHandling == NullValueHandling.Ignore)
            {
                continue;
            }

            string propertyName = _namingStrategy.GetPropertyName(property.Name, false);

            if (value is ResourceCollectionDto && typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
            {
                embeddedProperties.Add(propertyName, propertyValue);
            }
            else
            {
                JToken token = isPropertyValueNull ? JValue.CreateNull() : JToken.FromObject(propertyValue, serializer);
                jo.Add(propertyName, token);
            }
        }

        if (embeddedProperties.Any())
        {
            jo.Add(EmbeddedPropertyName, JToken.FromObject(embeddedProperties, serializer));
        }
    }
}