using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Shared.Serialization;

/// <summary>
///     JSON converter factory interface.
/// </summary>
public interface IJsonConverterFactory
{
    /// <summary>
    ///     Check if the given type is handled by this factory.
    /// </summary>
    /// <param name="objectType">Type to convert.</param>
    /// <returns>True if the factory can handle the given type.</returns>
    // ReSharper disable once InconsistentNaming
    bool CanConvert(Type objectType);

    /// <summary>
    ///     Instantiate a <see cref="JsonConverter" /> in function of the given type.
    /// </summary>
    /// <param name="configurationType">Type to convert.</param>
    /// <param name="namingStrategy">Naming strategy.</param>
    /// <returns>The JSON converter.</returns>
    JsonConverter CreateConverter(Type configurationType, NamingStrategy namingStrategy);
}