using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace BlazorBlocks.Model;

internal sealed class JsonSerializerTypeResolver : DefaultJsonTypeInfoResolver
{
    private readonly IReadOnlyList<BlockRegistration> _blockRegistrations;
    private readonly JsonPolymorphismOptions _polymorphismOptions;
    public JsonSerializerTypeResolver(IReadOnlyList<BlockRegistration> blockRegistrations)
    {
        _blockRegistrations = blockRegistrations;

        var jsonTypes = _blockRegistrations.Select(reg => new JsonDerivedType(reg.BlockModel, reg.BlockModel.Name)).ToList() ?? new List<JsonDerivedType>();

        _polymorphismOptions = new JsonPolymorphismOptions()
        {
            TypeDiscriminatorPropertyName = "$modelType",
            IgnoreUnrecognizedTypeDiscriminators = true,
            UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
        };

        foreach (var jsonType in jsonTypes)
        {
            _polymorphismOptions.DerivedTypes.Add(jsonType);
        }
    }

    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);

        if (!_blockRegistrations.Any()) return jsonTypeInfo;

        Type baseBlockType = typeof(EditorBlockBaseModel);
        if (jsonTypeInfo.Type == baseBlockType && jsonTypeInfo.PolymorphismOptions == null && !jsonTypeInfo.IsReadOnly)
        {
            jsonTypeInfo.PolymorphismOptions = _polymorphismOptions;
        }

        return jsonTypeInfo;
    }
}
