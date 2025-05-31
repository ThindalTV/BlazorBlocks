using BlazorBlocks.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorBlocks.Blocks.QuoteBlock;
public class QuoteBlockModel : BlazorBlocksEditorBlockBaseModel
{
    public override string EditorName => "Quote";

    public string Quote { get; set; } = string.Empty;

    public QuoteBlockModel() : base(typeof(QuoteBlockEditor))
    {
    }

    public override string Render()
    {
        return $"""<blockquote>{Quote}</blockquote>""";
    }
}
