using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DenuvoNetCredDecrypter.Domain
{
    /// <summary>
    /// Raw envelope returned by the credential service.
    /// </summary>
    public sealed class CredentialResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        // Kept as JToken because the service may return either a string or an array.
        [JsonProperty("errors")]
        public JToken? Errors { get; set; }

        // Kept as JToken because the service may return either a string or an array.
        [JsonProperty("messages")]
        public JToken? Messages { get; set; }

        [JsonProperty("result")]
        public CredentialResult? Result { get; set; }
    }
}
