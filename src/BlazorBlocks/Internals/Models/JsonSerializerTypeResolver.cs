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

    // Keyed by options as well as type: a JsonTypeInfo is bound to the options it was created for.
    private readonly Dictionary<(Type, JsonSerializerOptions), JsonTypeInfo> _types = new();
    private readonly object _typesLock = new();

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
        lock (_typesLock)
        {
            if (_types.TryGetValue((type, options), out var typeInfo))
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

            _types[(type, options)] = jsonTypeInfo;

            return jsonTypeInfo;
        }
    }
}
