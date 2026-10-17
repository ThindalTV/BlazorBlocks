using BlazorBlocks.Customization;

namespace BlazorBlocks.Blocks.TitleBlock;

public enum TitleSizeEnum
{
    Huge,
    Big,
    Medium,
    Small
}

public partial class TitleBlockModel : BaseBlockModel
{
    public override string EditorName => "Title";

    public TitleSizeEnum TitleSize { get; set; } = TitleSizeEnum.Huge;

    public string Title { get; set; } = string.Empty;

    public TitleBlockModel() : base(typeof(TitleEditorBlock))
    {
    }
}
