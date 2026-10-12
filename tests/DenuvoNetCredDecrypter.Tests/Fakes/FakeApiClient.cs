using System.Threading;
using System.Threading.Tasks;
using DenuvoNetCredDecrypter.Abstractions;
using DenuvoNetCredDecrypter.Domain;

namespace DenuvoNetCredDecrypter.Tests.Fakes
{
    internal sealed class FakeApiClient : ICredentialApiClient
    {
        private readonly ApiCallResult _result;

        public FakeApiClient(ApiCallResult result)
        {
            _result = result;
        }

        public CredentialRequest? LastRequest { get; private set; }

        public Task<ApiCallResult> RequestCredentialsAsync(CredentialRequest request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            return Task.FromResult(_result);
        }
    }
}
