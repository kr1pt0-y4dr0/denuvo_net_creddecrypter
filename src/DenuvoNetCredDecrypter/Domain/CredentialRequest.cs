using Newtonsoft.Json;

namespace DenuvoNetCredDecrypter.Domain
{
    /// <summary>
    /// Request payload sent to the credential service.
    /// </summary>
    public sealed class CredentialRequest
    {
        public CredentialRequest(string hwid, string login)
        {
            Hwid = hwid;
            Login = login;
        }

        [JsonProperty("act")]
        public string Act { get; } = "reqcreds";

        [JsonProperty("hwid")]
        public string Hwid { get; }

        [JsonProperty("login")]
        public string Login { get; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.None);
        }
    }
}
