namespace DenuvoNetCredDecrypter.Abstractions
{
    /// <summary>
    /// Decrypts the password ciphertext returned by the service.
    /// </summary>
    public interface ICredentialDecryptor
    {
        string Decrypt(string cipherBase64);
    }
}
