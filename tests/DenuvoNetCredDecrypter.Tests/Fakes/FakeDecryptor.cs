using DenuvoNetCredDecrypter.Abstractions;

namespace DenuvoNetCredDecrypter.Tests.Fakes
{
    internal sealed class FakeDecryptor : ICredentialDecryptor
    {
        private readonly string _plaintext;

        public FakeDecryptor(string plaintext)
        {
            _plaintext = plaintext;
        }

        public string? LastCipher { get; private set; }

        public string Decrypt(string cipherBase64)
        {
            LastCipher = cipherBase64;
            return _plaintext;
        }
    }
}
