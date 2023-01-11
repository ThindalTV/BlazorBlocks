using BlazorBlocks.Shared.BlockEditor.Blocks.Editors;

namespace BlazorBlocks.Shared.BlockEditor.Model.Editors;

public partial class TitleBlockModel : EditorBlockBaseModel
{
    public string Title { get; set; }
    public TitleBlockModel()
    {
        BlazorBlockEditor = typeof(TitleEditorBlock);
    }

    public override string Render()
    {
        return $"<h1>{Title}</h1>";
    }
}
