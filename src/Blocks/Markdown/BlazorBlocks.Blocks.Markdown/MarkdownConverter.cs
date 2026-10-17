using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace BlazorBlocks.Blocks.Markdown;

/// <summary>Converts Markdown to HTML with Markdig, with raw HTML disabled and script-capable link schemes removed.</summary>
public static class MarkdownConverter
{
    private static readonly string[] UnsafeSchemes = ["javascript:", "vbscript:", "data:"];

    private static readonly MarkdownPipeline Pipeline = BuildPipeline();

    private static MarkdownPipeline BuildPipeline()
    {
        var builder = new MarkdownPipelineBuilder().UseAdvancedExtensions().DisableHtml();

        // Auto-generated heading ids would collide when several Markdown blocks share a page.
        builder.Extensions.RemoveAll(e => e is Markdig.Extensions.AutoIdentifiers.AutoIdentifierExtension);
        return builder.Build();
    }

    public static string ToHtml(string? markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
        {
            return string.Empty;
        }

        var document = Markdig.Markdown.Parse(markdown, Pipeline);

        foreach (var link in document.Descendants<LinkInline>())
        {
            if (link.Url is { } url && IsUnsafe(url))
            {
                link.Url = "#";
            }
        }

        using var writer = new StringWriter();
        var renderer = new Markdig.Renderers.HtmlRenderer(writer);
        Pipeline.Setup(renderer);
        renderer.Render(document);
        return writer.ToString().TrimEnd();
    }

    private static bool IsUnsafe(string url)
    {
        // Browsers ignore whitespace and control characters inside the scheme, so strip them before checking.
        var cleaned = new string(url.Where(c => !char.IsWhiteSpace(c) && !char.IsControl(c)).ToArray());
        return UnsafeSchemes.Any(s => cleaned.StartsWith(s, StringComparison.OrdinalIgnoreCase));
    }
}
