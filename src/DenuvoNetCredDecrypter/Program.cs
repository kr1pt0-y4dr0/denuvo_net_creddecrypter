using System;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using DenuvoNetCredDecrypter.Abstractions;
using DenuvoNetCredDecrypter.App;
using DenuvoNetCredDecrypter.Configuration;
using DenuvoNetCredDecrypter.Infrastructure.Console;
using DenuvoNetCredDecrypter.Services;

namespace DenuvoNetCredDecrypter
{
    /// <summary>
    /// Composition root: wires up the object graph and runs the application.
    /// </summary>
    internal static class Program
    {
        private static async Task<int> Main()
        {
            System.Console.OutputEncoding = Encoding.UTF8;

            IUserInterface ui = new ConsoleUserInterface();

            try
            {
                AppOptions options = AppOptionsLoader.Load(AppContext.BaseDirectory);

                using (HttpClient httpClient = CreateHttpClient(options))
                {
                    var application = new CredentialApplication(
                        ui,
                        new HwidProvider(new WmiMachineInfoProvider(), new HwidCalculator()),
                        new CredentialApiClient(httpClient, options.ApiUrl),
                        new AesCredentialDecryptor(options.AesKeySeed),
                        new TextCredentialStore(AppContext.BaseDirectory));

                    int exitCode = await application.RunAsync().ConfigureAwait(false);
                    PauseIfInteractive(ui);
                    return exitCode;
                }
            }
            catch (ConfigurationException ex)
            {
                ui.ShowError(ex.Message);
                PauseIfInteractive(ui);
                return 2;
            }
            catch (Exception ex)
            {
                ui.ShowError("Unexpected error: " + ex.Message);
                ui.ShowDim(ex.ToString());
                PauseIfInteractive(ui);
                return 1;
            }
        }

        private static HttpClient CreateHttpClient(AppOptions options)
        {
            var handler = new HttpClientHandler
            {
                // The remote service uses a certificate that is not trusted by the
                // local machine, so validation is intentionally disabled.
                ServerCertificateCustomValidationCallback = (sender, certificate, chain, errors) => true,
                SslProtocols = SslProtocols.Tls12
            };

            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            client.DefaultRequestHeaders.UserAgent.ParseAdd(options.UserAgent);
            return client;
        }

        private static void PauseIfInteractive(IUserInterface ui)
        {
            if (!System.Console.IsInputRedirected && !System.Console.IsOutputRedirected)
            {
                ui.Pause();
            }
        }
    }
}
