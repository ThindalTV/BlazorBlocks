using BlazorBlocks.Shared.BlockEditor.Model;
using BlazorBlocks.Test.WASM.Shared.BlockEditor.Blocks.TitleBlock;

namespace BlazorBlocks.Shared.BlockEditor.Blocks.TitleBlock;

public partial class TitleBlockModel : EditorBlockBaseModel
{
    public string Title { get; set; } = string.Empty;

    public TitleBlockModel() : base(typeof(TitleEditorBlock))
    {
    }

    public override string Render()
    {
        return $"<h1>{Title}</h1>";
    }
}
