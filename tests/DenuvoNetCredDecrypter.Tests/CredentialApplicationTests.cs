using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DenuvoNetCredDecrypter.App;
using DenuvoNetCredDecrypter.Domain;
using DenuvoNetCredDecrypter.Tests.Fakes;
using Newtonsoft.Json.Linq;
using Xunit;

namespace DenuvoNetCredDecrypter.Tests
{
    public sealed class CredentialApplicationTests
    {
        private static HwidComputation DetectedHwid()
        {
            return new HwidComputation("cpu", "base", "concat", "MACHINE-HWID");
        }

        private static CredentialResponse CheckOk()
        {
            return new CredentialResponse { Success = true };
        }

        private static CredentialResponse SuccessResponse()
        {
            return new CredentialResponse
            {
                Success = true,
                Result = new CredentialResult
                {
                    AuthLogin = "alice",
                    AuthPasswordCipher = "CIPHER",
                    Type = "steam"
                }
            };
        }

        private static ApiCallResult Ok(CredentialResponse response)
        {
            return new ApiCallResult(HttpStatusCode.OK, "{}", response);
        }

        [Fact]
        public async Task Saves_the_credentials_and_returns_success()
        {
            var ui = new FakeUserInterface("id-1", "MACHINE-HWID", "my hardware");
            var api = new FakeApiClient(Ok(CheckOk()), Ok(SuccessResponse()));
            var decryptor = new FakeDecryptor("the-password");
            var store = new FakeCredentialStore { ExistsResult = false, PathResult = "id-1.txt" };

            var app = new CredentialApplication(ui, new FakeHwidProvider(DetectedHwid()), api, decryptor, store);
            var exitCode = await app.RunAsync();

            Assert.Equal(0, exitCode);
            Assert.NotNull(store.SavedRecord);
            Assert.Equal("id-1", store.SavedRecord!.Id);
            Assert.Equal("alice", store.SavedRecord.Login);
            Assert.Equal("the-password", store.SavedRecord.Password);
            Assert.Equal("MACHINE-HWID", store.SavedRecord.Hwid);
            Assert.Equal("my hardware", store.SavedRecord.HardwareInfo);
            Assert.Equal("CIPHER", decryptor.LastCipher);
            Assert.Single(ui.Successes);

            Assert.Equal(2, api.Requests.Count);
            Assert.Equal("checkAccount", api.Requests[0].Act);
            Assert.Equal("my hardware", api.Requests[0].Pc);
            Assert.Equal("reqcreds", api.Requests[1].Act);
        }

        [Fact]
        public async Task Warns_when_entered_hwid_differs_from_detected()
        {
            var ui = new FakeUserInterface("id-1", "SOMETHING-ELSE", "hw");
            var api = new FakeApiClient(Ok(CheckOk()), Ok(SuccessResponse()));
            var store = new FakeCredentialStore { PathResult = "id-1.txt" };

            var app = new CredentialApplication(ui, new FakeHwidProvider(DetectedHwid()), api, new FakeDecryptor("p"), store);
            await app.RunAsync();

            Assert.Contains(ui.Warnings, w => w.Contains("differs"));
        }

        [Fact]
        public async Task Stops_before_requesting_credentials_when_the_account_check_fails()
        {
            var checkFailure = new CredentialResponse
            {
                Success = false,
                Errors = JToken.Parse("{\"404\":\"ID id-1 Not found\"}")
            };
            var ui = new FakeUserInterface("id-1", "MACHINE-HWID", "hw");
            var api = new FakeApiClient(Ok(checkFailure));
            var store = new FakeCredentialStore();

            var app = new CredentialApplication(ui, new FakeHwidProvider(DetectedHwid()), api, new FakeDecryptor("p"), store);
            var exitCode = await app.RunAsync();

            Assert.Equal(1, exitCode);
            Assert.Null(store.SavedRecord);
            Assert.Single(api.Requests);
            Assert.Equal("checkAccount", api.Requests[0].Act);
            Assert.Single(ui.Errors);
        }

        [Fact]
        public async Task Returns_failure_and_does_not_save_when_server_refuses_credentials()
        {
            var ui = new FakeUserInterface("id-1", "MACHINE-HWID", "hw");
            var api = new FakeApiClient(Ok(CheckOk()), Ok(new CredentialResponse { Success = false }));
            var store = new FakeCredentialStore();

            var app = new CredentialApplication(ui, new FakeHwidProvider(DetectedHwid()), api, new FakeDecryptor("p"), store);
            var exitCode = await app.RunAsync();

            Assert.Equal(1, exitCode);
            Assert.Null(store.SavedRecord);
            Assert.Single(ui.Errors);
        }

        [Fact]
        public async Task Does_not_overwrite_when_user_declines()
        {
            var ui = new FakeUserInterface("id-1", "MACHINE-HWID", "hw") { ConfirmResult = false };
            var api = new FakeApiClient(Ok(CheckOk()), Ok(SuccessResponse()));
            var store = new FakeCredentialStore { ExistsResult = true, PathResult = "id-1.txt" };

            var app = new CredentialApplication(ui, new FakeHwidProvider(DetectedHwid()), api, new FakeDecryptor("p"), store);
            var exitCode = await app.RunAsync();

            Assert.Equal(1, exitCode);
            Assert.Null(store.SavedRecord);
        }
    }
}
