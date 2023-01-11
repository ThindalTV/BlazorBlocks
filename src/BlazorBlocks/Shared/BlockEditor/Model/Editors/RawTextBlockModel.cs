using BlazorBlocks.Shared.BlockEditor.Blocks.Editors;

namespace BlazorBlocks.Shared.BlockEditor.Model.Editors;

public class RawTextBlockModel : EditorBlockBaseModel
{
    public string Content { get; set; }
    public RawTextBlockModel()
	{
        BlazorBlockEditor = typeof(RawTextEditorBlock);
    }

    public override string Render()
    {
        return $"<div>{Content}</div>";
    }
}
