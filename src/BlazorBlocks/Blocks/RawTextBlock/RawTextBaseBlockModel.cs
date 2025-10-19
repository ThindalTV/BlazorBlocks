using BlazorBlocks.Customization;

namespace BlazorBlocks.Blocks.RawTextBlock;

public class RawTextBaseBlockModel : BaseBlockModel
{
    public override string EditorName => "Raw Text";

    public string Content { get; set; } = string.Empty;
    public RawTextBaseBlockModel() : base(typeof(RawTextEditorBlock))
    {
    }

    public override string Render()
    {
        return $"<div>{Content}</div>";
    }
}