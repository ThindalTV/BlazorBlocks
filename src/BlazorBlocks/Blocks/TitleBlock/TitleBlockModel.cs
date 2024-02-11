using BlazorBlocks.Model;

namespace BlazorBlocks.Blocks.TitleBlock;

public enum TitleSizeEnum
{
    Huge,
    Big,
    Medium,
    Small
}

public partial class TitleBlockModel : BlazorBlocksEditorBlockBaseModel
{
    public override string EditorName => "Title";

    public TitleSizeEnum TitleSize { get; set; } = TitleSizeEnum.Huge;

    public string Title { get; set; } = string.Empty;

    public TitleBlockModel() : base(typeof(TitleEditorBlock))
    {
    }

    public override string Render()
    {
        var size = TitleSize switch
        {
            TitleSizeEnum.Huge => "h1",
            TitleSizeEnum.Big => "h2",
            TitleSizeEnum.Medium => "h3",
            TitleSizeEnum.Small => "h4",
            _ => "h1"
        };
        return $"<{size}>{Title}</{size}>";
    }
}
