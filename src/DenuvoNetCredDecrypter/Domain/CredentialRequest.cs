using Newtonsoft.Json;

namespace DenuvoNetCredDecrypter.Domain
{
    /// <summary>
    /// Request payload sent to the credential service.
    /// </summary>
    /// <remarks>
    /// The service exposes a single endpoint whose behaviour is driven by the
    /// <c>act</c> field: <c>checkAccount</c> verifies that the credentials are
    /// available, while <c>reqcreds</c> returns them. Use the factory methods
    /// below to build the matching payload.
    /// </remarks>
    public sealed class CredentialRequest
    {
        private CredentialRequest(string act, string hwid, string login, string? pc)
        {
            Act = act;
            Hwid = hwid;
            Login = login;
            Pc = pc;
        }

        [JsonProperty("act")]
        public string Act { get; }

        [JsonProperty("hwid")]
        public string Hwid { get; }

        [JsonProperty("login")]
        public string Login { get; }

        // Only the checkAccount action carries the machine description.
        [JsonProperty("pc", NullValueHandling = NullValueHandling.Ignore)]
        public string? Pc { get; }

        /// <summary>
        /// Builds the "checkAccount" request that verifies the ID/HWID pair is
        /// known to the service before any credentials are requested.
        /// </summary>
        public static CredentialRequest ForCheckAccount(string hwid, string login, string pc)
        {
            return new CredentialRequest("checkAccount", hwid, login, pc);
        }

        /// <summary>
        /// Builds the "reqcreds" request that returns the stored credentials.
        /// </summary>
        public static CredentialRequest ForCredentials(string hwid, string login)
        {
            return new CredentialRequest("reqcreds", hwid, login, null);
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.None);
        }
    }
}
