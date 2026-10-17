using BlazorBlocks.Customization;

namespace BlazorBlocks.Blocks.QuoteBlock;
public class QuoteBlockModel : BaseBlockModel
{
    public override string EditorName => "Quote";

    public string Quote { get; set; } = string.Empty;

    public QuoteBlockModel() : base(typeof(QuoteBlockEditor))
    {
    }
}
