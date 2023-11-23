namespace Shared.Encryption;

/// <summary>
///     Symmetrical encryptor interface.
/// </summary>
public interface IEncryptor
{
    /// <summary>
    ///     Encrypt the given object.
    /// </summary>
    /// <param name="objectToEncrypt">Object to encrypt.</param>
    /// <returns>Encrypted object in string.</returns>
    string Encrypt(object objectToEncrypt);

    /// <summary>
    ///     Decrypt the given string to an object of the given type.
    /// </summary>
    /// <typeparam name="T">Object type to return.</typeparam>
    /// <param name="encryptedObject">Encrypted object.</param>
    /// <returns>Decrypted object.</returns>
    T Decrypt<T>(string encryptedObject);
}