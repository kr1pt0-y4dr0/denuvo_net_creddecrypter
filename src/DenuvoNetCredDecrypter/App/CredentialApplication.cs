using System;
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
        private const int TotalSteps = 5;
        private const string Title = "DenuvoNetCredDecrypter";
        private const string Version = "1.0.0";

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
            var id = _ui.PromptRequired("id (output file name and API login)", null);
            var hwid = _ui.PromptRequired("hwid", null);
            var hardwareInfo = _ui.PromptRequired("hardware info", null);

            if (!string.Equals(hwid, computation.Hwid, StringComparison.OrdinalIgnoreCase))
            {
                _ui.ShowWarning("The entered hwid differs from the HWID detected on this machine.");
            }

            var request = new CredentialRequest(hwid, id);
            _ui.ShowInfo("Request payload: " + request.ToJson());

            _ui.ShowStep(3, TotalSteps, "Requesting credentials from the server");
            ApiCallResult call;
            try
            {
                call = await _ui
                    .RunWithSpinnerAsync("POST /api.php", () => _apiClient.RequestCredentialsAsync(request, cancellationToken))
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _ui.ShowError("The request failed: " + ex.Message);
                return 1;
            }

            _ui.ShowField("HTTP status", ((int)call.StatusCode).ToString());
            _ui.ShowRawResponse(call.RawBody);

            CredentialResponse response = call.Response;
            if (!response.Success)
            {
                _ui.ShowError("The server refused the request.");
                if (response.Errors != null)
                {
                    _ui.ShowField("errors", response.Errors.ToString(Formatting.None));
                }

                if (response.Messages != null)
                {
                    _ui.ShowField("messages", response.Messages.ToString(Formatting.None));
                }

                return 1;
            }

            _ui.ShowStep(4, TotalSteps, "Decrypting the credentials");
            var login = response.Result?.AuthLogin ?? string.Empty;
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
            _ui.ShowField("password", new string('*', password.Length));

            _ui.ShowStep(5, TotalSteps, "Saving the result");
            var record = new CredentialRecord(id, login, password, hwid, hardwareInfo);
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
    }
}
