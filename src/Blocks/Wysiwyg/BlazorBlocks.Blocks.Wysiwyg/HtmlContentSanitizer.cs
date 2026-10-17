using Ganss.Xss;

namespace BlazorBlocks.Blocks.Wysiwyg;

/// <summary>
/// Makes editor HTML safe to emit into a page: scripts, event handlers, script URLs and unsafe CSS are removed,
/// while the formatting TinyMCE produces (including inline styles) is kept.
/// </summary>
public static class HtmlContentSanitizer
{
    private static readonly HtmlSanitizer Sanitizer = Create();

    private static HtmlSanitizer Create()
    {
        var sanitizer = new HtmlSanitizer();
        // TinyMCE expresses alignment, colours and font sizes as inline styles; only safe CSS properties survive.
        sanitizer.AllowedAttributes.Add("style");
        sanitizer.AllowedAttributes.Add("class");
        return sanitizer;
    }

    public static string Sanitize(string? html) =>
        string.IsNullOrWhiteSpace(html) ? string.Empty : Sanitizer.Sanitize(html);
}
