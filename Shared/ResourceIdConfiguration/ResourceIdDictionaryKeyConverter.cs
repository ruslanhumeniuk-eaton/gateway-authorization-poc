using Contracts.Application;
using Newtonsoft.Json;
using Shared.Encryption;

namespace Shared.ResourceIdConfiguration;

/// <summary>
///     <see cref="Dictionary{ResourceId, TValue}" /> converter.
/// </summary>
/// <typeparam name="TKey">Generic type of <see cref="ResourceId{T}" />.</typeparam>
/// <typeparam name="TValue">Type of dictionary value.</typeparam>
public class ResourceIdDictionaryKeyConverter<TKey, TValue> : JsonConverter<IDictionary<ResourceId<TKey>, TValue>>
{
    private readonly IEncryptor _encryptor;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ResourceIdDictionaryKeyConverter{TKey, TValue}" /> class.
    /// </summary>
    /// <param name="encryptor">Encryptor.</param>
    public ResourceIdDictionaryKeyConverter(IEncryptor encryptor)
    {
        _encryptor = encryptor;
    }

    /// <inheritdoc />
    public override IDictionary<ResourceId<TKey>, TValue> ReadJson(
        JsonReader reader,
        Type objectType,
        IDictionary<ResourceId<TKey>, TValue> existingValue,
        bool hasExistingValue,
        JsonSerializer serializer) => serializer
        .Deserialize<Dictionary<string, TValue>>(reader)
        .ToDictionary(x => new ResourceId<TKey>(_encryptor.Decrypt<TKey>(x.Key)), x => x.Value);

    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, IDictionary<ResourceId<TKey>, TValue> value, JsonSerializer serializer)
        => throw new NotImplementedException();
}