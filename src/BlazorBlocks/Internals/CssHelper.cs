using System.Text.RegularExpressions;

namespace BlazorBlocks.Internals;

public static class CssHelper
{
    // Strips any characters that are not valid in a CSS class attribute value.
    public static string SanitizeCssClass(string? value)
        => string.IsNullOrEmpty(value)
            ? string.Empty
            : Regex.Replace(value, @"[^a-zA-Z0-9\-_ ]", string.Empty);
}
