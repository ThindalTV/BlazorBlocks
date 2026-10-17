using System.Text.Json.Serialization;
using BlazorBlocks.Customization;

namespace BlazorBlocks.Blocks.Wysiwyg;

public class WysiwygBlockModel : BaseBlockModel
{
    public override string EditorName => "Rich text";

    /// <summary>The HTML as authored in the editor. Sanitized when rendered, never emitted as-is.</summary>
    public string Content { get; set; } = string.Empty;

    /// <summary><see cref="Content"/> with anything script-capable removed.</summary>
    [JsonIgnore]
    public string Html => HtmlContentSanitizer.Sanitize(Content);

    public WysiwygBlockModel() : base(typeof(WysiwygBlockEditor))
    {
    }
}
