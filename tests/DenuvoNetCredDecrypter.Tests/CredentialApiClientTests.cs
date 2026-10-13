using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DenuvoNetCredDecrypter.Domain;
using DenuvoNetCredDecrypter.Services;
using DenuvoNetCredDecrypter.Tests.Fakes;
using Newtonsoft.Json.Linq;
using Xunit;

namespace DenuvoNetCredDecrypter.Tests
{
    public sealed class CredentialApiClientTests
    {
        private const string ApiUrl = "https://example.test/api.php";

        [Fact]
        public async Task Posts_reqcreds_without_pc()
        {
            var handler = new StubHttpMessageHandler(_ => Json("{\"success\":true,\"result\":{}}"));
            var client = new CredentialApiClient(new HttpClient(handler), ApiUrl);

            await client.RequestCredentialsAsync(CredentialRequest.ForCredentials("HW", "user-1"), default);

            Assert.NotNull(handler.LastRequest);
            Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
            Assert.Equal(ApiUrl, handler.LastRequest.RequestUri!.ToString());
            Assert.Equal("application/json", handler.LastContentType);

            var body = JObject.Parse(handler.LastRequestBody!);
            Assert.Equal("reqcreds", (string?)body["act"]);
            Assert.Equal("HW", (string?)body["hwid"]);
            Assert.Equal("user-1", (string?)body["login"]);
            Assert.Null(body["pc"]);
        }

        [Fact]
        public async Task Posts_checkAccount_with_pc()
        {
            var handler = new StubHttpMessageHandler(_ => Json("{\"success\":true}"));
            var client = new CredentialApiClient(new HttpClient(handler), ApiUrl);

            await client.RequestCredentialsAsync(
                CredentialRequest.ForCheckAccount("HW", "user-1", "PC info"), default);

            var body = JObject.Parse(handler.LastRequestBody!);
            Assert.Equal("checkAccount", (string?)body["act"]);
            Assert.Equal("HW", (string?)body["hwid"]);
            Assert.Equal("user-1", (string?)body["login"]);
            Assert.Equal("PC info", (string?)body["pc"]);
        }

        [Fact]
        public async Task Parses_a_successful_response()
        {
            const string json = "{\"success\":true,\"errors\":null,\"messages\":null," +
                                "\"result\":{\"authlogin\":\"alice\",\"auth_pswd\":\"CIPHER\",\"type\":\"steam\"}}";
            var handler = new StubHttpMessageHandler(_ => Json(json));
            var client = new CredentialApiClient(new HttpClient(handler), ApiUrl);

            var result = await client.RequestCredentialsAsync(CredentialRequest.ForCredentials("HW", "user-1"), default);

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(json, result.RawBody);
            Assert.True(result.Response.Success);
            Assert.Equal("alice", result.Response.Result!.AuthLogin);
            Assert.Equal("CIPHER", result.Response.Result.AuthPasswordCipher);
            Assert.Equal("steam", result.Response.Result.Type);
        }

        [Fact]
        public async Task Parses_errors_returned_as_a_string()
        {
            var handler = new StubHttpMessageHandler(_ => Json("{\"success\":false,\"errors\":\"bad key\"}"));
            var client = new CredentialApiClient(new HttpClient(handler), ApiUrl);

            var result = await client.RequestCredentialsAsync(CredentialRequest.ForCredentials("HW", "user-1"), default);

            Assert.False(result.Response.Success);
            Assert.Equal(JTokenType.String, result.Response.Errors!.Type);
            Assert.Equal("bad key", result.Response.Errors.ToString());
        }

        [Fact]
        public async Task Parses_errors_returned_as_an_array()
        {
            var handler = new StubHttpMessageHandler(_ => Json("{\"success\":false,\"errors\":[\"e1\",\"e2\"]}"));
            var client = new CredentialApiClient(new HttpClient(handler), ApiUrl);

            var result = await client.RequestCredentialsAsync(CredentialRequest.ForCredentials("HW", "user-1"), default);

            Assert.False(result.Response.Success);
            Assert.Equal(JTokenType.Array, result.Response.Errors!.Type);
        }

        [Fact]
        public async Task Parses_errors_returned_as_an_object()
        {
            var handler = new StubHttpMessageHandler(_ => Json("{\"success\":false,\"errors\":{\"404\":\"ID nope Not found\"}}"));
            var client = new CredentialApiClient(new HttpClient(handler), ApiUrl);

            var result = await client.RequestCredentialsAsync(
                CredentialRequest.ForCheckAccount("HW", "user-1", "pc"), default);

            Assert.False(result.Response.Success);
            Assert.Equal(JTokenType.Object, result.Response.Errors!.Type);
            Assert.Equal("ID nope Not found", (string?)result.Response.Errors["404"]);
        }

        [Fact]
        public async Task Throws_when_the_body_is_not_json()
        {
            var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("<html>nope</html>", Encoding.UTF8, "text/html")
            });
            var client = new CredentialApiClient(new HttpClient(handler), ApiUrl);

            await Assert.ThrowsAsync<System.InvalidOperationException>(
                () => client.RequestCredentialsAsync(CredentialRequest.ForCredentials("HW", "user-1"), default));
        }

        private static HttpResponseMessage Json(string json)
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }
    }
}
