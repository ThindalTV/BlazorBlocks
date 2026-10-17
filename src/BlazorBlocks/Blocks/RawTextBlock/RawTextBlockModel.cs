using BlazorBlocks.Customization;

namespace BlazorBlocks.Blocks.RawTextBlock;

public class RawTextBlockModel : BaseBlockModel
{
    public override string EditorName => "Raw Text";

    public string Content { get; set; } = string.Empty;
    public RawTextBlockModel() : base(typeof(RawTextEditorBlock))
    {
    }
}
