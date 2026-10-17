using BlazorBlocks.Customization;

namespace BlazorBlocks.Sample.WASM.CustomBlocks.SampleBlock;

public class SampleBlockModel : BaseBlockModel
{
    public override string EditorName => "Custom Sample Block";

    public string? Content { get; set; }

    public SampleBlockModel() : base(typeof(SampleBlockEditor))
    {

    }
}
