using BlazorBlocks.Model;

namespace BlazorBlocks.Blocks.TitleBlock;

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
