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
