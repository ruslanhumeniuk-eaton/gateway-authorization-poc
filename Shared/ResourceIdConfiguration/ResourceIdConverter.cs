using Contracts.Application;
using Newtonsoft.Json;
using Shared.Encryption;

namespace Shared.ResourceIdConfiguration;

/// <summary>
///     <see cref="ResourceId{T}" /> JSON converter.
/// </summary>
/// <typeparam name="T">Type of the resource ID.</typeparam>
public class ResourceIdConverter<T> : JsonConverter<ResourceId<T>>
{
    private readonly IEncryptor _encryptor;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ResourceIdConverter{T}" /> class.
    /// </summary>
    /// <param name="encryptor">Encryptor.</param>
    public ResourceIdConverter(IEncryptor encryptor)
    {
        _encryptor = encryptor;
    }

    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, ResourceId<T> value, JsonSerializer serializer)
    {
        writer.WriteValue(_encryptor.Encrypt(value.Value));
    }

    /// <inheritdoc />
    public override ResourceId<T> ReadJson(JsonReader reader, Type objectType, ResourceId<T> existingValue, bool hasExistingValue,
        JsonSerializer serializer) => new(_encryptor.Decrypt<T>((string)reader.Value));
}