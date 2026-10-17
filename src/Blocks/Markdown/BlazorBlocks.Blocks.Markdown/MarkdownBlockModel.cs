using System.Text.Json.Serialization;
using BlazorBlocks.Customization;

namespace BlazorBlocks.Blocks.Markdown;

public class MarkdownBlockModel : BaseBlockModel
{
    public override string EditorName => "Markdown";

    public string Markdown { get; set; } = string.Empty;

    /// <summary>The Markdown converted to HTML. Raw HTML inside the Markdown is not passed through.</summary>
    [JsonIgnore]
    public string Html => MarkdownConverter.ToHtml(Markdown);

    public MarkdownBlockModel() : base(typeof(MarkdownBlockEditor))
    {
    }
}
