using Microsoft.AspNetCore.Components;
using System.Text.Json.Serialization;

namespace BlazorBlocks.Model;

public abstract class BlazorBlocksEditorBlockBaseModel : ComponentBase
{
    [JsonIgnore]
    public Type EditorType { get; }

    [JsonIgnore] public abstract string EditorName { get; }

    [JsonIgnore]
    public Dictionary<string, object> Parameters { get; set; }

    [JsonIgnore]
    public Action OnUpdated { get; set; }

    public BlazorBlocksEditorBlockBaseModel(Type editorType)
    {
        EditorType = editorType;
        Parameters = new Dictionary<string, object>() { { "Model", this } };
    }

    public abstract string Render();
}