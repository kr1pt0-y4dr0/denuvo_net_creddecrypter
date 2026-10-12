using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace DenuvoNetCredDecrypter.Infrastructure.Console
{
    /// <summary>
    /// A small animated console spinner that clears its line when disposed.
    /// It disables itself automatically when the output is redirected.
    /// </summary>
    internal sealed class Spinner : IDisposable
    {
        private static readonly char[] Frames = { '|', '/', '-', '\\' };

        private readonly string _message;
        private readonly CancellationTokenSource _cancellation = new CancellationTokenSource();
        private readonly Task _loop;
        private readonly bool _enabled;

        public Spinner(string message)
        {
            _message = message;
            _enabled = !System.Console.IsOutputRedirected;
            _loop = _enabled ? Task.Run(() => Spin(_cancellation.Token)) : Task.CompletedTask;
        }

        private void Spin(CancellationToken token)
        {
            var index = 0;
            while (!token.IsCancellationRequested)
            {
                var frame = Frames[index++ % Frames.Length];
                System.Console.Write("\r  " + _message + " " + frame);
                Thread.Sleep(90);
            }
        }

        public void Dispose()
        {
            _cancellation.Cancel();

            try
            {
                _loop.Wait();
            }
            catch (AggregateException ex)
            {
                // The render loop may fault if the console is torn down mid-write.
                Debug.WriteLine("Spinner render loop faulted: " + ex.Message);
            }

            System.Console.Write("\r" + new string(' ', _message.Length + 6) + "\r");
            _cancellation.Dispose();
        }
    }
}
