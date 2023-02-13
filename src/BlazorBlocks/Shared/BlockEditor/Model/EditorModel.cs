using BlazorBlocks.Shared.BlockEditor.Blocks;
using BlazorBlocks.Shared.BlockEditor.Blocks.ImageBlock;
using BlazorBlocks.Shared.BlockEditor.Blocks.RawTextBlock;
using BlazorBlocks.Shared.BlockEditor.Blocks.TitleBlock;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace BlazorBlocks.Shared.BlockEditor.Model;

public class EditorModel
{
    public List<EditorRowModel> Rows { get; set; }
    public List<BlockRegistration> BlockRegistrations { get; }

    private readonly JsonSerializerTypeResolver _jsonSerializerResolver;

    public EditorModel() : this(new List<BlockRegistration>())
    {

    }

    public EditorModel(List<BlockRegistration> editorRegistrations)
    {
        BlockRegistrations = editorRegistrations ?? new List<BlockRegistration>();

        BlockRegistrations.Add(new BlockRegistration("Title", typeof(TitleBlockModel), typeof(TitleEditorBlock)));
        BlockRegistrations.Add(new BlockRegistration("Raw text", typeof(RawTextBlockModel), typeof(RawTextEditorBlock)));
        BlockRegistrations.Add(new BlockRegistration("Image", typeof(ImageBlockModel), typeof(ImageEditorBlock)));

        _jsonSerializerResolver = new JsonSerializerTypeResolver(BlockRegistrations);

        Rows = new List<EditorRowModel>();
    }

    public string Render()
    {
        var sb = new StringBuilder();
        foreach (var row in Rows)
        {
            sb.Append(row.Render());
        }
        return sb.ToString();
    }

    public string Serialize()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions() { TypeInfoResolver = _jsonSerializerResolver });
    }

    public void Load(string data)
    {
        var model = JsonSerializer.Deserialize<EditorModel>(data, new JsonSerializerOptions() { TypeInfoResolver = _jsonSerializerResolver });
        Rows = model.Rows;
    }
}

public class BlockRegistration
{
    public string Name { get; }
    public Type BlockModel { get; }
    public Type EditorBlock { get; }

    public BlockRegistration(string name, Type modelType, Type editorType)
    {
        if (!modelType.IsSubclassOf(typeof(EditorBlockBaseModel)))
        {
            throw new ArgumentException($"{nameof(modelType)} is not derived from {nameof(EditorBlockBaseModel)}.");
        }

        Name = name;
        BlockModel = modelType;
        EditorBlock = editorType;
    }
}

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

public class PolymorphicJsonConverter<T> : JsonConverter<T>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(T).IsAssignableFrom(typeToConvert);
    }

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartObject();
        foreach (var property in value.GetType().GetProperties())
        {
            if (!property.CanRead)
                continue;
            var propertyValue = property.GetValue(value);
            writer.WritePropertyName(property.Name);
            JsonSerializer.Serialize(writer, propertyValue, options);
        }
        writer.WriteEndObject();
    }
}