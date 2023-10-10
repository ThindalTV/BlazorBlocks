using Microsoft.AspNetCore.Components;
using System.Text.Json.Serialization;

namespace BlazorBlocks.Shared.BlockEditor.Model;

public abstract class EditorBlockBaseModel : ComponentBase
{
    [JsonIgnore]
    public Type EditorType { get; }

    [JsonIgnore]
    public Dictionary<string, object> Parameters { get; set; }

    public EditorBlockBaseModel(Type editorType)
    {
        EditorType = editorType;
        Parameters = new Dictionary<string, object>() { { "Model", this } };
    }

    public abstract string Render();
}