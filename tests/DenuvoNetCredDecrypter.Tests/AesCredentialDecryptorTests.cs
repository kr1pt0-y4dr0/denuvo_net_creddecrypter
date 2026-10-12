using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using DenuvoNetCredDecrypter.Services;
using Xunit;

namespace DenuvoNetCredDecrypter.Tests
{
    public sealed class AesCredentialDecryptorTests
    {
        private const string Seed = "unit-test-seed";
        private const string Password = "s3cret-password";

        [Fact]
        public void Drops_the_first_byte_and_returns_the_utf8_payload()
        {
            var cipher = Encrypt(Seed, header: 0x02, payload: Password);

            var result = new AesCredentialDecryptor(Seed).Decrypt(cipher);

            Assert.Equal(Password, result);
        }

        [Fact]
        public void Works_when_payload_fills_a_whole_block()
        {
            var payload = new string('a', 31); // header + 31 = 32 bytes -> full padding block
            var cipher = Encrypt(Seed, header: 0x00, payload: payload);

            var result = new AesCredentialDecryptor(Seed).Decrypt(cipher);

            Assert.Equal(payload, result);
        }

        [Fact]
        public void Returns_empty_for_empty_input()
        {
            Assert.Equal(string.Empty, new AesCredentialDecryptor(Seed).Decrypt(string.Empty));
        }

        [Fact]
        public void Uses_a_key_derived_from_the_seed()
        {
            var otherSeedCipher = Encrypt("another-seed", header: 0x01, payload: Password);

            Assert.ThrowsAny<CryptographicException>(() => new AesCredentialDecryptor(Seed).Decrypt(otherSeedCipher));
        }

        private static string Encrypt(string seed, byte header, string payload)
        {
            byte[] key;
            using (var sha256 = SHA256.Create())
            {
                key = sha256.ComputeHash(Encoding.UTF8.GetBytes(seed));
            }

            var data = new List<byte> { header };
            data.AddRange(Encoding.UTF8.GetBytes(payload));

            var padding = 16 - (data.Count % 16);
            for (var i = 0; i < padding; i++)
            {
                data.Add((byte)padding);
            }

            using (var aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.None;
                aes.Key = key;
                aes.IV = new byte[16];

                using (var encryptor = aes.CreateEncryptor())
                {
                    var cipher = encryptor.TransformFinalBlock(data.ToArray(), 0, data.Count);
                    return Convert.ToBase64String(cipher);
                }
            }
        }
    }
}
