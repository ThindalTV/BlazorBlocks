using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using BlazorBlocks.Model.Registrations;

namespace BlazorBlocks.Model;

internal sealed class JsonSerializerTypeResolver : DefaultJsonTypeInfoResolver
{
    private readonly IReadOnlyList<BlazorBlocksBlockRegistration> _blockRegistrations;
    private readonly JsonPolymorphismOptions _polymorphismOptions;

    private readonly Dictionary<Type, JsonTypeInfo> _types;
    public JsonSerializerTypeResolver(IReadOnlyList<BlazorBlocksBlockRegistration> blockRegistrations)
    {
        _types = new Dictionary<Type, JsonTypeInfo>();

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
        if(_types.ContainsKey(type))
        {
            return _types[type];
        }

        JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);

        if (!_blockRegistrations.Any()) return jsonTypeInfo;

        Type baseBlockType = typeof(BlazorBlocksEditorBlockBaseModel);
        if (jsonTypeInfo.Type == baseBlockType && jsonTypeInfo.PolymorphismOptions == null && !jsonTypeInfo.IsReadOnly)
        {
            jsonTypeInfo.PolymorphismOptions = _polymorphismOptions;
        }

        _types.Add(type, jsonTypeInfo);

        return jsonTypeInfo;
    }
}
