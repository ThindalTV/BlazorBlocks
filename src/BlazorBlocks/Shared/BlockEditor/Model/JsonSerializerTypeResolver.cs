using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace BlazorBlocks.Shared.BlockEditor.Model;
internal sealed class JsonSerializerTypeResolver : DefaultJsonTypeInfoResolver
{
    private readonly List<BlockRegistration> _blockRegistrations;
    private readonly JsonPolymorphismOptions _polymorphismOptions;
    public JsonSerializerTypeResolver(List<BlockRegistration> blockRegistrations)
    {
        _blockRegistrations = blockRegistrations ?? new List<BlockRegistration>();

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

        Type baseModelType = typeof(EditorBlockBaseModel);
        if (jsonTypeInfo.Type == baseModelType)
        {
            jsonTypeInfo.PolymorphismOptions = _polymorphismOptions;
        }

        return jsonTypeInfo;
    }
}
