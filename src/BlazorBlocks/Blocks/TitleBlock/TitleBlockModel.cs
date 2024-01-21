using BlazorBlocks.Model;

namespace BlazorBlocks.Blocks.TitleBlock;

public partial class TitleBlockModel : BlazorBlocksEditorBlockBaseModel
{
    public override string EditorName => "Title";

    public string Title { get; set; } = string.Empty;

    public TitleBlockModel() : base(typeof(TitleEditorBlock))
    {
    }

    public override string Render()
    {
        return $"<h1>{Title}</h1>";
    }
}
