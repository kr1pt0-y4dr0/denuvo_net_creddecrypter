using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DenuvoNetCredDecrypter.Abstractions;
using DenuvoNetCredDecrypter.Domain;
using Newtonsoft.Json;

namespace DenuvoNetCredDecrypter.App
{
    /// <summary>
    /// Orchestrates the interactive credential retrieval flow.
    /// </summary>
    public sealed class CredentialApplication
    {
        private const int TotalSteps = 6;
        private const string Title = "DenuvoNetCredDecrypter";
        private static readonly string Version = ReadVersion();

        private readonly IUserInterface _ui;
        private readonly IHwidProvider _hwidProvider;
        private readonly ICredentialApiClient _apiClient;
        private readonly ICredentialDecryptor _decryptor;
        private readonly ICredentialStore _store;

        public CredentialApplication(
            IUserInterface ui,
            IHwidProvider hwidProvider,
            ICredentialApiClient apiClient,
            ICredentialDecryptor decryptor,
            ICredentialStore store)
        {
            _ui = ui ?? throw new ArgumentNullException(nameof(ui));
            _hwidProvider = hwidProvider ?? throw new ArgumentNullException(nameof(hwidProvider));
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            _decryptor = decryptor ?? throw new ArgumentNullException(nameof(decryptor));
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public async Task<int> RunAsync(CancellationToken cancellationToken = default)
        {
            _ui.ShowBanner(Title, Version);

            _ui.ShowStep(1, TotalSteps, "Detecting this machine's HWID");
            HwidComputation computation = _hwidProvider.GetCurrent();
            _ui.ShowField("cpuId", computation.CpuId);
            _ui.ShowField("baseId", computation.BaseId);
            _ui.ShowField("concat", computation.Concatenated);
            _ui.ShowField("HWID", computation.Hwid);

            _ui.ShowStep(2, TotalSteps, "Entering the request data");
            var id = _ui.PromptRequired("ID (From your Order Info)", null);
            var hwid = _ui.PromptRequired("hwid", null);
            var hardwareInfo = _ui.PromptRequired("hardware info", null);

            if (!string.Equals(hwid, computation.Hwid, StringComparison.OrdinalIgnoreCase))
            {
                _ui.ShowWarning("The entered hwid differs from the HWID detected on this machine.");
            }

            var checkRequest = CredentialRequest.ForCheckAccount(hwid, id, hardwareInfo);
            _ui.ShowInfo("Request payload: " + checkRequest.ToJson());

            _ui.ShowStep(3, TotalSteps, "Checking that the credentials are available");
            ApiCallResult? checkCall = await SendAsync(checkRequest, cancellationToken);
            if (checkCall == null)
            {
                return 1;
            }

            if (!checkCall.Response.Success)
            {
                _ui.ShowError("The account check did not pass.");
                ShowResponseFailures(checkCall.Response);
                return 1;
            }

            _ui.ShowSuccess("The account is available.");

            var request = CredentialRequest.ForCredentials(hwid, id);
            _ui.ShowInfo("Request payload: " + request.ToJson());

            _ui.ShowStep(4, TotalSteps, "Requesting credentials from the server");
            ApiCallResult? call = await SendAsync(request, cancellationToken);
            if (call == null)
            {
                return 1;
            }

            CredentialResponse response = call.Response;
            if (!response.Success)
            {
                _ui.ShowError("The server refused the request.");
                ShowResponseFailures(response);
                return 1;
            }

            if (response.Result == null)
            {
                _ui.ShowError("The server reported success but returned no credentials.");
                ShowResponseFailures(response);
                return 1;
            }

            _ui.ShowStep(5, TotalSteps, "Decrypting the credentials");
            var login = response.Result?.AuthLogin ?? string.Empty;
            var type = response.Result?.Type ?? string.Empty;
            string password;
            try
            {
                password = _decryptor.Decrypt(response.Result?.AuthPasswordCipher ?? string.Empty);
            }
            catch (Exception ex)
            {
                _ui.ShowError("Failed to decrypt the password: " + ex.Message);
                return 1;
            }

            _ui.ShowField("login", login);
            _ui.ShowField("password", password);
            _ui.ShowField("type", type);

            _ui.ShowStep(6, TotalSteps, "Saving the result");
            var record = new CredentialRecord(id, login, password, hwid, hardwareInfo, type);
            var path = _store.GetPath(id);
            if (_store.Exists(id))
            {
                if (!_ui.Confirm($"The file '{path}' already exists. Overwrite?", false))
                {
                    _ui.ShowWarning("Aborted. The existing file was left untouched.");
                    return 1;
                }
            }

            try
            {
                _store.Save(record, overwrite: true);
            }
            catch (Exception ex)
            {
                _ui.ShowError("Failed to write the file: " + ex.Message);
                return 1;
            }

            _ui.ShowSuccess("Saved credentials to " + path);
            return 0;
        }

        /// <summary>
        /// Sends one request to the service, reporting the HTTP status and the
        /// raw body. Returns <c>null</c> when the call itself failed.
        /// </summary>
        private async Task<ApiCallResult?> SendAsync(CredentialRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var call = await _ui
                    .RunWithSpinnerAsync("POST /api.php", () => _apiClient.RequestCredentialsAsync(request, cancellationToken))
                    .ConfigureAwait(false);

                _ui.ShowField("HTTP status", ((int)call.StatusCode).ToString());
                _ui.ShowRawResponse(call.RawBody);
                return call;
            }
            catch (Exception ex)
            {
                _ui.ShowError("The request failed: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Prints the "errors" and "messages" members of a failed response. Both
        /// may be a string, an array or (for errors) an object keyed by code.
        /// </summary>
        private void ShowResponseFailures(CredentialResponse response)
        {
            if (response.Errors != null)
            {
                _ui.ShowField("errors", response.Errors.ToString(Formatting.None));
            }

            if (response.Messages != null)
            {
                _ui.ShowField("messages", response.Messages.ToString(Formatting.None));
            }
        }

        /// <summary>
        /// Reads the product version from the assembly metadata so it stays in
        /// sync with the version stamped at build time (for example by the
        /// release workflow). Falls back to the assembly version, then "0.0.0".
        /// </summary>
        private static string ReadVersion()
        {
            var assembly = typeof(CredentialApplication).Assembly;

            var informational = assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion;

            if (!string.IsNullOrEmpty(informational))
            {
                // Strip the source-revision suffix ("+<sha>") when present.
                var value = informational!;
                var separator = value.IndexOf('+');
                return separator >= 0 ? value.Substring(0, separator) : value;
            }

            return assembly.GetName().Version?.ToString(3) ?? "0.0.0";
        }
    }
}
