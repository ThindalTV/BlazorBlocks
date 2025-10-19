using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using BlazorBlocks.Customization;
using BlazorBlocks.Services.Registrations;

namespace BlazorBlocks.Internals.Models;

internal sealed class JsonSerializerTypeResolver : DefaultJsonTypeInfoResolver
{
    private readonly IReadOnlyList<BlockRegistration> _blockRegistrations;
    private readonly JsonPolymorphismOptions _polymorphismOptions;

    private readonly Dictionary<Type, JsonTypeInfo> _types = new Dictionary<Type, JsonTypeInfo>();
    public JsonSerializerTypeResolver(IReadOnlyList<BlockRegistration> blockRegistrations)
    {
        _blockRegistrations = blockRegistrations;

        var jsonTypes = _blockRegistrations.Select(reg => new JsonDerivedType(reg.BlockModel, reg.BlockModel.Name)).ToList();

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
        if(_types.TryGetValue(type, out var typeInfo))
        {
            return typeInfo;
        }

        var jsonTypeInfo = base.GetTypeInfo(type, options);

        if (!_blockRegistrations.Any())
        {
            return jsonTypeInfo;
        }

        Type baseBlockType = typeof(BaseBlockModel);
        if (jsonTypeInfo.Type == baseBlockType && jsonTypeInfo.PolymorphismOptions == null && !jsonTypeInfo.IsReadOnly)
        {
            jsonTypeInfo.PolymorphismOptions = _polymorphismOptions;
        }

        _types.Add(type, jsonTypeInfo);

        return jsonTypeInfo;
    }
}
