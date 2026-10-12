using Newtonsoft.Json;

namespace DenuvoNetCredDecrypter.Configuration
{
    /// <summary>
    /// Strongly typed representation of <c>appsettings.json</c>.
    /// </summary>
    public sealed class AppOptions
    {
        [JsonProperty("apiUrl")]
        public string ApiUrl { get; set; } = string.Empty;

        [JsonProperty("userAgent")]
        public string UserAgent { get; set; } = string.Empty;

        [JsonProperty("aesKeySeed")]
        public string AesKeySeed { get; set; } = string.Empty;
    }
}
