using System;
using System.Threading.Tasks;
using DenuvoNetCredDecrypter.Abstractions;

namespace DenuvoNetCredDecrypter.Infrastructure.Console
{
    /// <summary>
    /// Console implementation of <see cref="IUserInterface"/>.
    /// </summary>
    public sealed class ConsoleUserInterface : IUserInterface
    {
        public void ShowBanner(string title, string version)
        {
            System.Console.WriteLine();
            ConsoleTheme.WriteLineColor(ConsoleTheme.Accent, "  " + title + "  v" + version);
            ConsoleTheme.WriteLineColor(ConsoleTheme.Dim, "  " + new string('-', title.Length + version.Length + 6));
            System.Console.WriteLine();
        }

        public void ShowStep(int index, int total, string title)
        {
            System.Console.WriteLine();
            ConsoleTheme.WriteColor(ConsoleTheme.Step, $"  [{index}/{total}] ");
            ConsoleTheme.WriteLineColor(ConsoleTheme.Accent, title);
        }

        public void ShowInfo(string message)
        {
            Prefix("  i  ", ConsoleTheme.Accent, message);
        }

        public void ShowSuccess(string message)
        {
            Prefix("  +  ", ConsoleTheme.Success, message);
        }

        public void ShowWarning(string message)
        {
            Prefix("  !  ", ConsoleTheme.Warning, message);
        }

        public void ShowError(string message)
        {
            Prefix("  x  ", ConsoleTheme.Error, message);
        }

        public void ShowDim(string message)
        {
            ConsoleTheme.WriteLineColor(ConsoleTheme.Dim, "     " + message);
        }

        public void ShowField(string label, string value)
        {
            ConsoleTheme.WriteColor(ConsoleTheme.Label, "     " + label + ": ");
            System.Console.WriteLine(value);
        }

        public void ShowRawResponse(string body)
        {
            System.Console.WriteLine();
            ConsoleTheme.WriteLineColor(ConsoleTheme.Dim, "     raw response:");
            ConsoleTheme.WriteLineColor(ConsoleTheme.Dim, "     " + body);
            System.Console.WriteLine();
        }

        public string Prompt(string label, string? hint)
        {
            ConsoleTheme.WriteColor(ConsoleTheme.Accent, "  ? ");
            System.Console.Write(label);
            if (!string.IsNullOrEmpty(hint))
            {
                ConsoleTheme.WriteColor(ConsoleTheme.Dim, " (" + hint + ")");
            }

            System.Console.Write(": ");
            return System.Console.ReadLine() ?? string.Empty;
        }

        public string PromptRequired(string label, string? hint)
        {
            while (true)
            {
                var value = Prompt(label, hint);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value.Trim();
                }

                ShowWarning("A value is required.");
            }
        }

        public bool Confirm(string question, bool defaultValue)
        {
            var suffix = defaultValue ? "[Y/n]" : "[y/N]";
            while (true)
            {
                ConsoleTheme.WriteColor(ConsoleTheme.Accent, "  ? ");
                System.Console.Write(question + " " + suffix + ": ");
                var answer = (System.Console.ReadLine() ?? string.Empty).Trim();

                if (answer.Length == 0)
                {
                    return defaultValue;
                }

                if (answer.Equals("y", StringComparison.OrdinalIgnoreCase)
                    || answer.Equals("yes", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                if (answer.Equals("n", StringComparison.OrdinalIgnoreCase)
                    || answer.Equals("no", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                ShowWarning("Please answer 'y' or 'n'.");
            }
        }

        public async Task<T> RunWithSpinnerAsync<T>(string message, Func<Task<T>> action)
        {
            if (System.Console.IsOutputRedirected)
            {
                ShowInfo(message);
                return await action().ConfigureAwait(false);
            }

            using (new Spinner(message))
            {
                return await action().ConfigureAwait(false);
            }
        }

        public void Pause()
        {
            System.Console.WriteLine();
            ConsoleTheme.WriteColor(ConsoleTheme.Dim, "  Press Enter to exit...");
            System.Console.ReadLine();
        }

        private static void Prefix(string prefix, ConsoleColor color, string message)
        {
            ConsoleTheme.WriteColor(color, prefix);
            System.Console.WriteLine(message);
        }
    }
}
