using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DenuvoNetCredDecrypter.Abstractions;
using DenuvoNetCredDecrypter.Domain;
using Newtonsoft.Json;

namespace DenuvoNetCredDecrypter.Services
{
    /// <summary>
    /// POSTs JSON to the credential service and parses its JSON envelope.
    /// </summary>
    public sealed class CredentialApiClient : ICredentialApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        public CredentialApiClient(HttpClient httpClient, string apiUrl)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _apiUrl = string.IsNullOrWhiteSpace(apiUrl)
                ? throw new ArgumentException("The API url must not be empty.", nameof(apiUrl))
                : apiUrl;
        }

        public async Task<ApiCallResult> RequestCredentialsAsync(CredentialRequest request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            using (var content = new StringContent(request.ToJson(), Encoding.UTF8, "application/json"))
            using (var response = await _httpClient.PostAsync(_apiUrl, content, cancellationToken).ConfigureAwait(false))
            {
                var rawBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                CredentialResponse parsed;
                try
                {
                    parsed = JsonConvert.DeserializeObject<CredentialResponse>(rawBody) ?? new CredentialResponse();
                }
                catch (JsonException ex)
                {
                    throw new InvalidOperationException(
                        $"The server response could not be parsed as JSON: {ex.Message}", ex);
                }

                return new ApiCallResult(response.StatusCode, rawBody, parsed);
            }
        }
    }
}
