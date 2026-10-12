using System;
using System.Threading.Tasks;

namespace DenuvoNetCredDecrypter.Abstractions
{
    /// <summary>
    /// All console interaction goes through this abstraction so that the
    /// application logic stays free of <c>System.Console</c>.
    /// </summary>
    public interface IUserInterface
    {
        void ShowBanner(string title, string version);

        void ShowStep(int index, int total, string title);

        void ShowInfo(string message);

        void ShowSuccess(string message);

        void ShowWarning(string message);

        void ShowError(string message);

        void ShowDim(string message);

        void ShowField(string label, string value);

        void ShowRawResponse(string body);

        string Prompt(string label, string? hint);

        string PromptRequired(string label, string? hint);

        bool Confirm(string question, bool defaultValue);

        Task<T> RunWithSpinnerAsync<T>(string message, Func<Task<T>> action);

        void Pause();
    }
}
