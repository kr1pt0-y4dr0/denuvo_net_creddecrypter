using System.Net;
using System.Threading.Tasks;
using DenuvoNetCredDecrypter.App;
using DenuvoNetCredDecrypter.Domain;
using DenuvoNetCredDecrypter.Tests.Fakes;
using Xunit;

namespace DenuvoNetCredDecrypter.Tests
{
    public sealed class CredentialApplicationTests
    {
        private static HwidComputation DetectedHwid()
        {
            return new HwidComputation("cpu", "base", "concat", "MACHINE-HWID");
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
            var api = new FakeApiClient(Ok(SuccessResponse()));
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
        }

        [Fact]
        public async Task Warns_when_entered_hwid_differs_from_detected()
        {
            var ui = new FakeUserInterface("id-1", "SOMETHING-ELSE", "hw");
            var api = new FakeApiClient(Ok(SuccessResponse()));
            var store = new FakeCredentialStore { PathResult = "id-1.txt" };

            var app = new CredentialApplication(ui, new FakeHwidProvider(DetectedHwid()), api, new FakeDecryptor("p"), store);
            await app.RunAsync();

            Assert.Contains(ui.Warnings, w => w.Contains("differs"));
        }

        [Fact]
        public async Task Returns_failure_and_does_not_save_when_server_refuses()
        {
            var ui = new FakeUserInterface("id-1", "MACHINE-HWID", "hw");
            var api = new FakeApiClient(Ok(new CredentialResponse { Success = false }));
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
            var api = new FakeApiClient(Ok(SuccessResponse()));
            var store = new FakeCredentialStore { ExistsResult = true, PathResult = "id-1.txt" };

            var app = new CredentialApplication(ui, new FakeHwidProvider(DetectedHwid()), api, new FakeDecryptor("p"), store);
            var exitCode = await app.RunAsync();

            Assert.Equal(1, exitCode);
            Assert.Null(store.SavedRecord);
        }
    }
}
