using System.Text.RegularExpressions;

namespace BlazorBlocks.Tests.Model;

/// <summary>
/// The rendered-output stylesheet is loaded on the host's own pages (Bootstrap, Tailwind, ...),
/// so it must not restyle anything that is not a BlazorBlocks element (issue #42).
/// </summary>
public class OutputCssTests
{
    private static string WwwRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            var candidate = Path.Combine(dir.FullName, "src", "BlazorBlocks", "wwwroot");
            if (Directory.Exists(candidate))
            {
                return candidate;
            }

            dir = dir.Parent;
        }

        throw new DirectoryNotFoundException("Could not find src/BlazorBlocks/wwwroot.");
    }

    private static string Css(string file) =>
        Regex.Replace(File.ReadAllText(Path.Combine(WwwRoot(), file)), @"/\*.*?\*/", "", RegexOptions.Singleline);

    private static IEnumerable<string> Selectors(string css) =>
        Regex.Matches(css, @"([^{};]+)\{")
            .Select(m => m.Groups[1].Value.Trim())
            .Where(s => !s.StartsWith('@'))
            .SelectMany(s => s.Split(','))
            .Select(s => s.Trim());

    [Fact]
    public void OutputStylesheet_ImportsNoEditorStyles()
    {
        var css = Css("blazorblocks.css");

        Assert.DoesNotContain("blazorblocks-editor.css", css);
        Assert.DoesNotContain("blazorblocks-components.css", css);
    }

    [Theory]
    [InlineData("blazorblocks-layout.css")]
    [InlineData("blazorblocks-utilities.css")]
    [InlineData("blazorblocks-variables.css")]
    public void OutputStylesheets_OnlyTargetBlazorBlocksClasses(string file)
    {
        foreach (var selector in Selectors(Css(file)))
        {
            Assert.True(selector == ":root" || selector.StartsWith(".bb-") || selector.StartsWith("[class*=\"bb-"),
                $"'{selector}' in {file} can match elements outside BlazorBlocks.");
        }
    }

    [Fact]
    public void Columns_AreFullWidthUntilTheMediumBreakpoint()
    {
        var css = Css("blazorblocks-layout.css");
        var firstWidthRule = css.IndexOf(".bb-layout-column--6", StringComparison.Ordinal);
        var mediaQuery = css.IndexOf("@media (min-width: 768px)", StringComparison.Ordinal);

        Assert.True(mediaQuery >= 0 && mediaQuery < firstWidthRule, "Column widths must only apply inside the min-width media query.");
    }
}
