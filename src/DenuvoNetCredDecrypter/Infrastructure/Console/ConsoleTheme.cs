using System;

namespace DenuvoNetCredDecrypter.Infrastructure.Console
{
    /// <summary>
    /// Central place for console colours.
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
    }
}
