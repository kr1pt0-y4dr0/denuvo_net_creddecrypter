using System.IO;
using Newtonsoft.Json;

namespace DenuvoNetCredDecrypter.Configuration
{
    /// <summary>
    /// Loads and validates <see cref="AppOptions"/> from <c>appsettings.json</c>
    /// located next to the executable.
    /// </summary>
    public static class AppOptionsLoader
    {
        public const string FileName = "appsettings.json";

        public static AppOptions Load(string baseDirectory)
        {
            var path = Path.Combine(baseDirectory, FileName);
            if (!File.Exists(path))
            {
                throw new ConfigurationException(
                    $"Configuration file '{FileName}' was not found next to the executable ({baseDirectory}).");
            }

            AppOptions? options;
            try
            {
                options = JsonConvert.DeserializeObject<AppOptions>(File.ReadAllText(path));
            }
            catch (JsonException ex)
            {
                throw new ConfigurationException($"Configuration file '{FileName}' is not valid JSON: {ex.Message}");
            }

            if (options is null)
            {
                throw new ConfigurationException($"Configuration file '{FileName}' is empty.");
            }

            if (string.IsNullOrWhiteSpace(options.ApiUrl))
            {
                throw new ConfigurationException("'apiUrl' must be set in appsettings.json.");
            }

            if (string.IsNullOrWhiteSpace(options.UserAgent))
            {
                throw new ConfigurationException("'userAgent' must be set in appsettings.json.");
            }

            if (string.IsNullOrWhiteSpace(options.AesKeySeed))
            {
                throw new ConfigurationException("'aesKeySeed' must be set in appsettings.json.");
            }

            return options;
        }
    }
}
