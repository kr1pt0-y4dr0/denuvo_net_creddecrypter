using System;
using System.IO;
using System.Text;
using DenuvoNetCredDecrypter.Abstractions;
using DenuvoNetCredDecrypter.Domain;

namespace DenuvoNetCredDecrypter.Services
{
    /// <summary>
    /// Writes credentials to <c>&lt;id&gt;.txt</c> as labelled lines.
    /// </summary>
    public sealed class TextCredentialStore : ICredentialStore
    {
        private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();
        private static readonly UTF8Encoding Utf8NoBom = new UTF8Encoding(false);

        private readonly string _directory;

        public TextCredentialStore(string directory)
        {
            _directory = string.IsNullOrWhiteSpace(directory)
                ? Directory.GetCurrentDirectory()
                : directory;
        }

        public string GetPath(string id)
        {
            return Path.Combine(_directory, Sanitize(id) + ".txt");
        }

        public bool Exists(string id)
        {
            return File.Exists(GetPath(id));
        }

        public void Save(CredentialRecord record, bool overwrite)
        {
            var path = GetPath(record.Id);
            if (File.Exists(path) && !overwrite)
            {
                throw new IOException($"The file '{path}' already exists.");
            }

            var builder = new StringBuilder();
            builder.AppendLine("login: " + record.Login);
            builder.AppendLine("password: " + record.Password);
            builder.AppendLine("hwid: " + record.Hwid);
            builder.AppendLine("hardware info: " + record.HardwareInfo);

            File.WriteAllText(path, builder.ToString(), Utf8NoBom);
        }

        internal static string Sanitize(string id)
        {
            var trimmed = (id ?? string.Empty).Trim();
            var builder = new StringBuilder(trimmed.Length);
            foreach (var ch in trimmed)
            {
                builder.Append(Array.IndexOf(InvalidFileNameChars, ch) >= 0 ? '_' : ch);
            }

            var result = builder.ToString();
            if (result.Length == 0)
            {
                throw new ArgumentException("The id must not be empty.", nameof(id));
            }

            return result;
        }
    }
}
