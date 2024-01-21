using BlazorBlocks.Model;

namespace BlazorBlocks.Test.WASM.CustomBlocks.TestBlock;

public class TestBlockModel : BlazorBlocksEditorBlockBaseModel
{
    public override string EditorName => "Custom Test Block";

    public string? Content { get; set; }

    public TestBlockModel() : base(typeof(TestBlockEditor))
    {
        
    }

    public override string Render()
    {
        return $"""<div><h1 style="color: red">Test block</h1> {Content}</div>""";
    }
}
