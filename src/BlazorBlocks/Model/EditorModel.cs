using System.Text.Json.Serialization;

namespace BlazorBlocks.Model;

public class EditorModel
{
    [JsonIgnore]
    public Guid Id { get; } = Guid.NewGuid();

    [JsonIgnore]
    public EditorModel? ParentModel {get; set; }

    [JsonIgnore]
    public bool Collapsed { get; set; }
}