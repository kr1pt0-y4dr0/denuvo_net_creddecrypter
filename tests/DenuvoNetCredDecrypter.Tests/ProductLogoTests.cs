using System.Linq;
using DenuvoNetCredDecrypter.Infrastructure.Console;
using Xunit;

namespace DenuvoNetCredDecrypter.Tests
{
    public sealed class ProductLogoTests
    {
        [Fact]
        public void Renders_full_name_when_it_fits()
        {
            var lines = new ProductLogo().Render("DenuvoNetCredDecrypter", 200);

            Assert.True(lines.Count >= 4);
            Assert.True(lines.Max(l => l.Length) <= 200);
        }

        [Fact]
        public void Falls_back_to_monogram_when_too_narrow()
        {
            var lines = new ProductLogo().Render("DenuvoNetCredDecrypter", 40);

            Assert.True(lines.Max(l => l.Length) <= 40);
        }

        [Theory]
        [InlineData("DenuvoNetCredDecrypter", "DNCD")]
        [InlineData("alllower", "alllower")]
        public void Monogram_uses_upper_case_letters(string input, string expected)
        {
            Assert.Equal(expected, ProductLogo.Monogram(input));
        }

        [Fact]
        public void Drops_blank_lines_and_dedents()
        {
            var lines = new ProductLogo().Render("DN", 200);

            Assert.DoesNotContain(lines, l => l.Trim().Length == 0);
        }
    }
}
