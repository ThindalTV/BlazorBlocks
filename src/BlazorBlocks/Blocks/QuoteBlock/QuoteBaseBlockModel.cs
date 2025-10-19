using BlazorBlocks.Customization;

namespace BlazorBlocks.Blocks.QuoteBlock;
public class QuoteBaseBlockModel : BaseBlockModel
{
    public override string EditorName => "Quote";

    public string Quote { get; set; } = string.Empty;

    public QuoteBaseBlockModel() : base(typeof(QuoteBlockEditor))
    {
    }

    public override string Render()
    {
        return $"""<blockquote>{Quote}</blockquote>""";
    }
}
