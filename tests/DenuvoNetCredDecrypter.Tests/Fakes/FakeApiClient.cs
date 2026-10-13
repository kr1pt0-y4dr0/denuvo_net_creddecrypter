using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DenuvoNetCredDecrypter.Abstractions;
using DenuvoNetCredDecrypter.Domain;

namespace DenuvoNetCredDecrypter.Tests.Fakes
{
    internal sealed class FakeApiClient : ICredentialApiClient
    {
        private readonly Queue<ApiCallResult> _results;

        public FakeApiClient(params ApiCallResult[] results)
        {
            _results = new Queue<ApiCallResult>(results);
        }

        public List<CredentialRequest> Requests { get; } = new List<CredentialRequest>();

        public CredentialRequest? LastRequest => Requests.Count > 0 ? Requests[Requests.Count - 1] : null;

        public Task<ApiCallResult> RequestCredentialsAsync(CredentialRequest request, CancellationToken cancellationToken)
        {
            Requests.Add(request);

            // Use the next canned result, reusing the last one for any further calls.
            var result = _results.Count > 1 ? _results.Dequeue() : _results.Peek();
            return Task.FromResult(result);
        }
    }
}
