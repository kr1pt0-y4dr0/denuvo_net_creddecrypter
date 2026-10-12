using System.Net;

namespace DenuvoNetCredDecrypter.Domain
{
    /// <summary>
    /// Outcome of a call to the credential service, including the raw body for display.
    /// </summary>
    public sealed class ApiCallResult
    {
        public ApiCallResult(HttpStatusCode statusCode, string rawBody, CredentialResponse response)
        {
            StatusCode = statusCode;
            RawBody = rawBody;
            Response = response;
        }

        public HttpStatusCode StatusCode { get; }

        public string RawBody { get; }

        public CredentialResponse Response { get; }
    }
}
