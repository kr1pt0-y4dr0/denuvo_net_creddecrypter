using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DenuvoNetCredDecrypter.Tests.Fakes
{
    /// <summary>
    /// Captures the outgoing request and returns a canned response, so tests
    /// never touch the network.
    /// </summary>
    internal sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;

        public StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
        {
            _responder = responder;
        }

        public HttpRequestMessage? LastRequest { get; private set; }

        public string? LastRequestBody { get; private set; }

        public string? LastContentType { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;

            if (request.Content != null)
            {
                LastRequestBody = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
                LastContentType = request.Content.Headers.ContentType?.MediaType;
            }

            return _responder(request);
        }
    }
}
