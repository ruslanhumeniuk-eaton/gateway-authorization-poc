using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;

namespace Shared.Encryption;

/// <summary>
///     Provide symmetrical encryption with AES 256.
/// </summary>
public class Encryptor : IEncryptor
{
    private readonly ICryptoTransform _decryptor;
    private readonly ICryptoTransform _encryptor;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Encryptor" /> class.
    ///     If null parameters, new key and initialization vector are used.
    /// </summary>
    /// <param name="aesKey">256 bits AES key.</param>
    /// <param name="aesInitializationVector">Initialization vector.</param>
    public Encryptor(string aesKey, string aesInitializationVector)
    {
        if (string.IsNullOrWhiteSpace(aesKey) || string.IsNullOrWhiteSpace(aesInitializationVector))
        {
            (aesKey, aesInitializationVector) = GetKeyInitializationVector();
        }

        Aes cipher = CreateCipher(aesKey);
        cipher.IV = System.Convert.FromBase64String(aesInitializationVector);

        _encryptor = cipher.CreateEncryptor();
        _decryptor = cipher.CreateDecryptor();
    }

    /// <inheritdoc />
    public string Encrypt(object objectToEncrypt)
    {
        byte[] plaintext = Encoding.UTF8.GetBytes(objectToEncrypt?.ToString() ?? string.Empty);
        byte[] cipherText = _encryptor.TransformFinalBlock(plaintext, 0, plaintext.Length);

        return Base64UrlEncoder.Encode(cipherText);
    }

    /// <inheritdoc />
    public T Decrypt<T>(string encryptedObject)
    {
        byte[] encryptedBytes = Base64UrlEncoder.DecodeBytes(encryptedObject);
        byte[] plainBytes = _decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
        string value = Encoding.UTF8.GetString(plainBytes);
        return Convert<T>(value);
    }

    /// <summary>
    ///     Get pair of 256 bits AES encryption key and initialization vector.
    /// </summary>
    /// <returns>Key and initialization vector.</returns>
    public (string key, string initializationVector) GetKeyInitializationVector()
    {
        string key = GetBase64EncodedRandomString(32); // 256
        Aes cipher = CreateCipher(key);
        string ivBase64 = System.Convert.ToBase64String(cipher.IV);
        return (key, ivBase64);
    }

    /// <summary>
    ///     Parse the given string into the given type.
    /// </summary>
    /// <typeparam name="T">Object type.</typeparam>
    /// <param name="input">String to parse.</param>
    /// <returns>Resulting object.</returns>
    private T Convert<T>(string input)
    {
        try
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));

            return (T)converter.ConvertFromString(input);
        }
        catch (NotSupportedException)
        {
            return default;
        }
    }

    /// <summary>
    ///     Generate a base 64 encoded random string of the given length in bytes.
    /// </summary>
    /// <param name="length">Bytes length random string generated.</param>
    /// <returns>Base 64 encoded random string.</returns>
    private string GetBase64EncodedRandomString(int length)
    {
        string base64 = System.Convert.ToBase64String(GenerateRandomBytes(length));
        return base64;
    }

    /// <summary>
    ///     Create a new cipher with the given key.
    /// </summary>
    /// <param name="keyBase64">Key base 64 encoded.</param>
    /// <returns>AES cipher.</returns>
    private Aes CreateCipher(string keyBase64)
    {
        // Default values: Keysize 256, Padding PKC27
        var cipher = Aes.Create();
        cipher.Mode = CipherMode.CBC; // Ensure the integrity of the ciphertext if using CBC

        cipher.Padding = PaddingMode.PKCS7;
        cipher.Key = System.Convert.FromBase64String(keyBase64);

        return cipher;
    }

    /// <summary>
    ///     Generate a random bytes array of the given length.
    /// </summary>
    /// <param name="length">Length of the array.</param>
    /// <returns>Random bytes array.</returns>
    private byte[] GenerateRandomBytes(int length)
    {
        var byteArray = new byte[length];
        RandomNumberGenerator.Fill(byteArray);
        return byteArray;
    }
}