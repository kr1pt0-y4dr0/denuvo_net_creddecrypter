using System;
using System.Security.Cryptography;
using System.Text;
using DenuvoNetCredDecrypter.Abstractions;

namespace DenuvoNetCredDecrypter.Services
{
    /// <summary>
    /// AES-256-CBC decoder. The key is the SHA-256 hash of the configured seed,
    /// the IV is all zeroes, padding is PKCS7 and the first plaintext byte is
    /// dropped (it is a header, not part of the password).
    /// </summary>
    public sealed class AesCredentialDecryptor : ICredentialDecryptor
    {
        private static readonly byte[] Iv = new byte[16];
        private readonly byte[] _key;

        public AesCredentialDecryptor(string keySeed)
        {
            if (string.IsNullOrEmpty(keySeed))
            {
                throw new ArgumentException("The AES key seed must not be empty.", nameof(keySeed));
            }

            using (var sha256 = SHA256.Create())
            {
                _key = sha256.ComputeHash(Encoding.UTF8.GetBytes(keySeed));
            }
        }

        public string Decrypt(string cipherBase64)
        {
            if (string.IsNullOrEmpty(cipherBase64))
            {
                return string.Empty;
            }

            var cipher = Convert.FromBase64String(cipherBase64);

            using (var aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = _key;
                aes.IV = Iv;

                using (var decryptor = aes.CreateDecryptor())
                {
                    var plain = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);
                    return plain.Length == 0
                        ? string.Empty
                        : Encoding.UTF8.GetString(plain, 1, plain.Length - 1);
                }
            }
        }
    }
}
