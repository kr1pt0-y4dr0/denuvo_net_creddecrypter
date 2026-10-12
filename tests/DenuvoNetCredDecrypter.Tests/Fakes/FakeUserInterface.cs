using System.Collections.Generic;
using System.Threading.Tasks;
using DenuvoNetCredDecrypter.Abstractions;

namespace DenuvoNetCredDecrypter.Tests.Fakes
{
    internal sealed class FakeUserInterface : IUserInterface
    {
        private readonly Queue<string> _answers;

        public FakeUserInterface(params string[] answers)
        {
            _answers = new Queue<string>(answers);
        }

        public List<string> Steps { get; } = new List<string>();

        public List<string> Infos { get; } = new List<string>();

        public List<string> Successes { get; } = new List<string>();

        public List<string> Warnings { get; } = new List<string>();

        public List<string> Errors { get; } = new List<string>();

        public string? LastRawResponse { get; private set; }

        public bool ConfirmResult { get; set; } = true;

        public void ShowBanner(string title, string version)
        {
        }

        public void ShowStep(int index, int total, string title) => Steps.Add(title);

        public void ShowInfo(string message) => Infos.Add(message);

        public void ShowSuccess(string message) => Successes.Add(message);

        public void ShowWarning(string message) => Warnings.Add(message);

        public void ShowError(string message) => Errors.Add(message);

        public void ShowDim(string message)
        {
        }

        public void ShowField(string label, string value)
        {
        }

        public void ShowRawResponse(string body) => LastRawResponse = body;

        public string Prompt(string label, string? hint) => _answers.Count > 0 ? _answers.Dequeue() : string.Empty;

        public string PromptRequired(string label, string? hint) => Prompt(label, hint);

        public bool Confirm(string question, bool defaultValue) => ConfirmResult;

        public Task<T> RunWithSpinnerAsync<T>(string message, System.Func<Task<T>> action) => action();

        public void Pause()
        {
        }
    }
}
