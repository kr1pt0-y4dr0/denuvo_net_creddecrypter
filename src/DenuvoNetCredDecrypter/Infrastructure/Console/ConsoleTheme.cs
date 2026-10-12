using System;
using System.Collections.Generic;

namespace DenuvoNetCredDecrypter.Infrastructure.Console
{
    /// <summary>
    /// Central place for console colours and logo rendering.
    /// </summary>
    public static class ConsoleTheme
    {
        public static readonly ConsoleColor Accent = ConsoleColor.Cyan;
        public static readonly ConsoleColor Step = ConsoleColor.DarkCyan;
        public static readonly ConsoleColor Success = ConsoleColor.Green;
        public static readonly ConsoleColor Warning = ConsoleColor.Yellow;
        public static readonly ConsoleColor Error = ConsoleColor.Red;
        public static readonly ConsoleColor Dim = ConsoleColor.DarkGray;
        public static readonly ConsoleColor Label = ConsoleColor.Gray;

        private static bool? _ansi;

        /// <summary>
        /// True when the terminal accepts ANSI escape sequences.
        /// </summary>
        public static bool SupportsAnsi
        {
            get
            {
                if (_ansi == null)
                {
                    _ansi = VirtualTerminal.TryEnable();
                }

                return _ansi.Value;
            }
        }

        public static void WriteColor(ConsoleColor color, string text)
        {
            var previous = System.Console.ForegroundColor;
            try
            {
                System.Console.ForegroundColor = color;
                System.Console.Write(text);
            }
            finally
            {
                System.Console.ForegroundColor = previous;
            }
        }

        public static void WriteLineColor(ConsoleColor color, string text)
        {
            WriteColor(color, text + Environment.NewLine);
        }

        /// <summary>
        /// Writes pre-rendered logo lines with a vertical truecolour gradient.
        /// Falls back to a flat <see cref="Accent"/> colour when the terminal
        /// does not support ANSI escape sequences.
        /// </summary>
        public static void WriteGradient(IReadOnlyList<string> lines)
        {
            if (lines == null || lines.Count == 0)
            {
                return;
            }

            for (var i = 0; i < lines.Count; i++)
            {
                if (!SupportsAnsi)
                {
                    WriteLineColor(Accent, lines[i]);
                    continue;
                }

                var t = lines.Count == 1 ? 0d : (double)i / (lines.Count - 1);
                var (r, g, b) = Lerp((0x22, 0xD3, 0xEE), (0x8B, 0x5C, 0xF6), t);
                System.Console.WriteLine($"\u001b[38;2;{r};{g};{b}m{lines[i]}\u001b[0m");
            }
        }

        private static (int R, int G, int B) Lerp((int R, int G, int B) from, (int R, int G, int B) to, double t)
        {
            return (
                (int)Math.Round(from.R + ((to.R - from.R) * t)),
                (int)Math.Round(from.G + ((to.G - from.G) * t)),
                (int)Math.Round(from.B + ((to.B - from.B) * t)));
        }
    }
}
