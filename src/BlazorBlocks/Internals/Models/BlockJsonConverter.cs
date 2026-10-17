using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using BlazorBlocks.Blocks.UnknownBlock;
using BlazorBlocks.Customization;
using BlazorBlocks.Services.Registrations;

namespace BlazorBlocks.Internals.Models;

/// <summary>
/// Reads and writes blocks with a <c>$modelType</c> discriminator taken from the registered blocks.
/// A block whose type is not registered is kept as an <see cref="UnknownBlockModel"/> and written back unchanged.
/// </summary>
internal sealed class BlockJsonConverter : JsonConverter<BaseBlockModel>
{
    internal const string DiscriminatorName = "$modelType";

    private readonly IReadOnlyList<BlockRegistration> _blockRegistrations;

    public BlockJsonConverter(IReadOnlyList<BlockRegistration> blockRegistrations)
    {
        _blockRegistrations = blockRegistrations;
    }

    // Only the base type: concrete block types use the default converter, so this does not recurse.
    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(BaseBlockModel);

    public override BaseBlockModel? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        var node = JsonNode.Parse(ref reader) as JsonObject
            ?? throw new JsonException("A block must be a JSON object.");

        var typeName = node[DiscriminatorName] is JsonValue value && value.TryGetValue<string>(out var name) ? name : null;
        var registration = _blockRegistrations.FirstOrDefault(r => r.BlockModel.Name == typeName);

        if (registration == null)
        {
            return new UnknownBlockModel(typeName, node);
        }

        return (BaseBlockModel?)node.Deserialize(registration.BlockModel, options);
    }

    public override void Write(Utf8JsonWriter writer, BaseBlockModel value, JsonSerializerOptions options)
    {
        if (value is UnknownBlockModel unknown)
        {
            unknown.Json.WriteTo(writer);
            return;
        }

        var valueType = value.GetType();
        var registration = _blockRegistrations.FirstOrDefault(r => r.BlockModel == valueType)
            ?? throw new NotSupportedException($"No block is registered for model type '{valueType.Name}', so it cannot be saved.");

        var properties = JsonSerializer.SerializeToNode(value, valueType, options) as JsonObject
            ?? throw new JsonException($"Block '{valueType.Name}' did not serialize to a JSON object.");

        writer.WriteStartObject();
        writer.WriteString(DiscriminatorName, registration.BlockModel.Name);
        foreach (var (propertyName, propertyValue) in properties)
        {
            writer.WritePropertyName(propertyName);
            if (propertyValue == null)
            {
                writer.WriteNullValue();
            }
            else
            {
                propertyValue.WriteTo(writer, options);
            }
        }

        writer.WriteEndObject();
    }
}
