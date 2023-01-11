using Microsoft.AspNetCore.Components;

namespace BlazorBlocks.Shared.BlockEditor.Model;

public abstract class EditorBlockBaseModel : ComponentBase
{
    public Type BlazorBlockEditor { get; set; }
    public Dictionary<string, object> Parameters { get; set; }

    public EditorBlockBaseModel()
    {
        Parameters = new Dictionary<string, object>() { { "Model", this } };
    }

    abstract public string Render();
}
