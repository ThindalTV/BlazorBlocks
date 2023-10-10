using BlazorBlocks.Model;

namespace BlazorBlocks.Blocks.RawTextBlock;

public class RawTextBlockModel : EditorBlockBaseModel
{
    public override string EditorName => "Raw Text";

    public string Content { get; set; } = string.Empty;
    public RawTextBlockModel() : base(typeof(RawTextEditorBlock))
    {
    }

    public override string Render()
    {
        return $"<div>{Content}</div>";
    }
}