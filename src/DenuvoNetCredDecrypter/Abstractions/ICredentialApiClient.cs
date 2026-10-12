using System.Threading;
using System.Threading.Tasks;
using DenuvoNetCredDecrypter.Domain;

namespace DenuvoNetCredDecrypter.Abstractions
{
    /// <summary>
    /// Talks to the remote credential service.
    /// </summary>
    public interface ICredentialApiClient
    {
        Task<ApiCallResult> RequestCredentialsAsync(CredentialRequest request, CancellationToken cancellationToken);
    }
}
