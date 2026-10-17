using System.Text.Json.Nodes;
using BlazorBlocks.Customization;

namespace BlazorBlocks.Blocks.UnknownBlock;

/// <summary>
/// Stands in for a block whose type is no longer registered, for example because it was removed or renamed.
/// It keeps the original JSON and writes it back unchanged on save, and the rendered output skips it.
/// </summary>
public class UnknownBlockModel : BaseBlockModel
{
    public override string EditorName => "Unknown";

    /// <summary>The <c>$modelType</c> the saved block had, or null if it had none.</summary>
    public string? OriginalType { get; }

    /// <summary>The block's original JSON.</summary>
    public JsonObject Json { get; }

    public UnknownBlockModel(string? originalType, JsonObject json) : base(typeof(UnknownBlockEditor))
    {
        OriginalType = originalType;
        Json = json;
    }
}
