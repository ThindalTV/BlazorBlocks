using System.Xml;
using System.Xml.Linq;

namespace BlazorBlocks.Blocks.SvgBlock;

/// <summary>
/// Makes untrusted SVG markup safe to emit inline into a page: removes scripts, event handlers,
/// embedded foreign content and non-local references.
/// </summary>
public static class SvgSanitizer
{
    private static readonly HashSet<string> BlockedElements = new(StringComparer.OrdinalIgnoreCase)
    {
        "script", "foreignObject", "iframe", "object", "embed", "style", "set", "animate",
    };

    /// <summary>
    /// Parses and sanitizes SVG markup.
    /// </summary>
    /// <returns>The safe markup, or null when <paramref name="markup"/> is empty, not well-formed XML, or not an <c>svg</c> document.</returns>
    public static string? Sanitize(string? markup) => Sanitize(markup, out _);

    /// <inheritdoc cref="Sanitize(string?)"/>
    /// <param name="markup">The SVG markup.</param>
    /// <param name="error">Why the markup was rejected, or null when it is valid or empty.</param>
    public static string? Sanitize(string? markup, out string? error)
    {
        error = null;
        if (string.IsNullOrWhiteSpace(markup))
        {
            return null;
        }

        XDocument doc;
        try
        {
            var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null };
            using var reader = XmlReader.Create(new StringReader(markup), settings);
            doc = XDocument.Load(reader);
        }
        catch (XmlException e)
        {
            error = e.Message;
            return null;
        }

        var root = doc.Root;
        if (root is null || !string.Equals(root.Name.LocalName, "svg", StringComparison.OrdinalIgnoreCase))
        {
            error = "The root element must be <svg>.";
            return null;
        }

        Clean(root);
        return root.ToString(SaveOptions.DisableFormatting);
    }

    private static void Clean(XElement element)
    {
        element.Elements().Where(e => BlockedElements.Contains(e.Name.LocalName)).Remove();

        foreach (var attribute in element.Attributes().ToList())
        {
            if (!IsSafe(attribute))
            {
                attribute.Remove();
            }
        }

        foreach (var child in element.Elements())
        {
            Clean(child);
        }
    }

    private static bool IsSafe(XAttribute attribute)
    {
        var name = attribute.Name.LocalName;
        var value = attribute.Value.Trim();

        if (name.StartsWith("on", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (name.Equals("href", StringComparison.OrdinalIgnoreCase))
        {
            return value.StartsWith('#')
                || value.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                || value.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
                || value.StartsWith("data:image/", StringComparison.OrdinalIgnoreCase) && !value.StartsWith("data:image/svg", StringComparison.OrdinalIgnoreCase);
        }

        if (value.Contains("javascript:", StringComparison.OrdinalIgnoreCase)
            || name.Equals("style", StringComparison.OrdinalIgnoreCase)
               && (value.Contains("url(", StringComparison.OrdinalIgnoreCase) || value.Contains("expression", StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        return true;
    }
}
