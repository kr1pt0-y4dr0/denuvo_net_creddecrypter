using Newtonsoft.Json;

namespace DenuvoNetCredDecrypter.Domain
{
    /// <summary>
    /// The <c>result</c> object of a successful server response.
    /// </summary>
    public sealed class CredentialResult
    {
        [JsonProperty("authlogin")]
        public string? AuthLogin { get; set; }

        [JsonProperty("auth_pswd")]
        public string? AuthPasswordCipher { get; set; }

        [JsonProperty("type")]
        public string? Type { get; set; }
    }
}
