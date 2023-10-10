using BlazorBlocks.Shared.BlockEditor.Model;
using BlazorBlocks.Test.WASM.Shared.BlockEditor.Blocks.RawTextBlock;

namespace BlazorBlocks.Shared.BlockEditor.Blocks.RawTextBlock;

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