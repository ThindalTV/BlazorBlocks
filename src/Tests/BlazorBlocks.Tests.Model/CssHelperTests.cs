using BlazorBlocks.Internals;

namespace BlazorBlocks.Tests.Model;

public class CssHelperTests
{
    // ── Valid class names ────────────────────────────────────────────────────

    [Theory]
    [InlineData("row",        "row")]
    [InlineData("bb-layout",  "bb-layout")]
    [InlineData("col_12",     "col_12")]
    [InlineData("foo bar",    "foo bar")]
    public void SanitizeCssClass_ValidInput_ReturnsUnchanged(string input, string expected)
    {
        // Act
        var result = CssHelper.SanitizeCssClass(input);

        // Assert
        Assert.Equal(expected, result);
    }

    // ── Invalid characters are stripped ─────────────────────────────────────

    [Theory]
    [InlineData("fo\"o",      "foo")]   // double quote
    [InlineData("fo'o",       "foo")]   // single quote
    [InlineData("<script>",   "script")]// angle brackets
    [InlineData("a=b",        "ab")]    // equals sign
    [InlineData("cls(1)",     "cls1")]  // parentheses
    [InlineData("a;b",        "ab")]    // semicolon
    [InlineData("a!b",        "ab")]    // exclamation mark
    [InlineData("a@b",        "ab")]    // at sign
    public void SanitizeCssClass_InvalidCharacters_AreStripped(string input, string expected)
    {
        // Act
        var result = CssHelper.SanitizeCssClass(input);

        // Assert
        Assert.Equal(expected, result);
    }

    // ── Null / empty guards ──────────────────────────────────────────────────

    [Fact]
    public void SanitizeCssClass_NullInput_ReturnsEmptyString()
    {
        // Act
        var result = CssHelper.SanitizeCssClass(null);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void SanitizeCssClass_EmptyStringInput_ReturnsEmptyString()
    {
        // Act
        var result = CssHelper.SanitizeCssClass(string.Empty);

        // Assert
        Assert.Equal(string.Empty, result);
    }
}
