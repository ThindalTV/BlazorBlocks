using BlazorBlocks.BlockEditor.Model;

namespace BlazorBlocks.Blocks.RawTextBlock;

public class RawTextBlockModel : EditorBlockBaseModel
{
    public string Content { get; set; } = string.Empty;
    public RawTextBlockModel() : base(typeof(RawTextEditorBlock))
    {
    }

    public override string Render()
    {
        return $"<div>{Content}</div>";
    }
}