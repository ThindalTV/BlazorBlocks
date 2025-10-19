using System.Text.Json.Serialization;

namespace BlazorBlocks.Customization;

public abstract class BaseBlockModel
{
    [JsonIgnore]
    public Type DukaType { get; }

    [JsonIgnore] public abstract string EditorName { get; }

    [JsonIgnore]
    public Dictionary<string, object> Parameters { get; set; }

    public BaseBlockModel(Type dukaType)
    {
        DukaType = dukaType;
        Parameters = new Dictionary<string, object>() { { "Model", this } };
    }

    public abstract string Render();

    public virtual Task OnValuesChanged() => Task.CompletedTask;
}