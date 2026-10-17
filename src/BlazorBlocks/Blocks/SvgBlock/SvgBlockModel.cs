using System.Text.Json.Serialization;
using BlazorBlocks.Customization;

namespace BlazorBlocks.Blocks.SvgBlock;

public class SvgBlockModel : BaseBlockModel
{
    /// <summary>SVG files larger than this are rejected by the editor.</summary>
    public const long MaxUploadBytes = 256 * 1024;

    public override string EditorName => "SVG";

    /// <summary>The SVG markup, as XML. Sanitized when rendered, never trusted as stored.</summary>
    public string Markup { get; set; } = string.Empty;

    /// <summary>Accessible description of the graphic. Without it the SVG is hidden from assistive technology.</summary>
    public string? AltText { get; set; } = string.Empty;

    /// <summary>The sanitized markup, or null when <see cref="Markup"/> is empty or not a valid SVG document.</summary>
    [JsonIgnore]
    public string? SafeMarkup => SvgSanitizer.Sanitize(Markup);

    public SvgBlockModel() : base(typeof(SvgBlockEditor))
    {
    }
}
