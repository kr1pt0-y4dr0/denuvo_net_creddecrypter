using System;
using System.Runtime.InteropServices;

namespace DenuvoNetCredDecrypter.Infrastructure.Console
{
    /// <summary>
    /// Enables ANSI escape-sequence processing on the Windows console. Modern
    /// terminals (Windows Terminal, VS Code, ConEmu) already enable it; this
    /// keeps legacy hosts consistent and fails quietly when unsupported.
    /// </summary>
    internal static class VirtualTerminal
    {
        private const int StdOutputHandle = -11;
        private const uint EnableVirtualTerminalProcessing = 0x0004;

        public static bool TryEnable()
        {
            if (System.Console.IsOutputRedirected)
            {
                return false;
            }

            var handle = GetStdHandle(StdOutputHandle);
            if (handle == IntPtr.Zero || handle == new IntPtr(-1))
            {
                return false;
            }

            return GetConsoleMode(handle, out var mode)
                && SetConsoleMode(handle, mode | EnableVirtualTerminalProcessing);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);
    }
}
