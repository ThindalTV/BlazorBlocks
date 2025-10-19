using System.Text.Json.Serialization;

namespace BlazorBlocks.Internals.Models;

public abstract class BaseModel
{
    [JsonIgnore]
    public Guid Id { get; } = Guid.NewGuid();

    [JsonIgnore]
    public BaseModel? ParentModel {get; set; }

    [JsonIgnore]
    public bool Collapsed { get; set; }
}