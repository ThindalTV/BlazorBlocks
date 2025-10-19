using BlazorBlocks.Customization;

namespace BlazorBlocks.Test.WASM.CustomBlocks.TestBlock;

public class TestBaseBlockModel : BaseBlockModel
{
    public override string EditorName => "Custom Test BaseBlock";

    public string? Content { get; set; }

    public TestBaseBlockModel() : base(typeof(TestBlockEditor))
    {
        
    }

    public override string Render()
    {
        return $"""<div><h1 style="color: red">Test block</h1> {Content}</div>""";
    }
}
