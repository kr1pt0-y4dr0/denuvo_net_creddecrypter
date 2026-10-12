using System.Collections.Generic;
using System.Linq;
using Figgle.Fonts;

namespace DenuvoNetCredDecrypter.Infrastructure.Console
{
    /// <summary>
    /// Renders the product logo as FIGlet art using the Figgle package
    /// (<c>Figgle.Fonts</c> bundles 250+ FIGlet fonts).
    /// </summary>
    /// <remarks>
    /// The full product name is rendered in a compact font; when it would not
    /// fit the available console width the renderer falls back to a monogram
    /// built from the name's upper-case letters.
    /// </remarks>
    public sealed class ProductLogo
    {
        public IReadOnlyList<string> Render(string productName, int maxWidth)
        {
            var name = productName ?? string.Empty;
            var fullName = ToLines(FiggleFonts.Small.Render(name));
            if (Fits(fullName, maxWidth))
            {
                return fullName;
            }

            return ToLines(FiggleFonts.Standard.Render(Monogram(name)));
        }

        /// <summary>
        /// Builds a short monogram from the upper-case letters of <paramref name="name"/>
        /// (for example "DenuvoNetCredDecrypter" becomes "DNC").
        /// </summary>
        public static string Monogram(string? name)
        {
            var letters = new string((name ?? string.Empty).Where(char.IsUpper).ToArray());
            return letters.Length > 0 ? letters : (name ?? string.Empty).Trim();
        }

        private static bool Fits(IReadOnlyList<string> lines, int maxWidth)
        {
            foreach (var line in lines)
            {
                if (line.Length > maxWidth)
                {
                    return false;
                }
            }

            return true;
        }

        private static List<string> ToLines(string art)
        {
            var lines = (art ?? string.Empty)
                .Replace("\r\n", "\n")
                .Split('\n')
                .ToList();

            // Drop blank lines at the top and bottom of the block.
            while (lines.Count > 0 && lines[0].Trim().Length == 0)
            {
                lines.RemoveAt(0);
            }

            while (lines.Count > 0 && lines[lines.Count - 1].Trim().Length == 0)
            {
                lines.RemoveAt(lines.Count - 1);
            }

            return Dedent(lines);
        }

        private static List<string> Dedent(List<string> lines)
        {
            var indent = int.MaxValue;
            foreach (var line in lines)
            {
                if (line.Trim().Length == 0)
                {
                    continue;
                }

                var leading = line.Length - line.TrimStart().Length;
                if (leading < indent)
                {
                    indent = leading;
                }
            }

            if (indent == int.MaxValue || indent == 0)
            {
                return lines;
            }

            for (var i = 0; i < lines.Count; i++)
            {
                lines[i] = lines[i].Length >= indent ? lines[i].Substring(indent) : string.Empty;
            }

            return lines;
        }
    }
}
