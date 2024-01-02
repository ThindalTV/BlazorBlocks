using BlazorBlocks.Blocks.ImageBlock;
using BlazorBlocks.Blocks.TitleBlock;
using BlazorBlocks.Blocks.RawTextBlock;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components;

namespace BlazorBlocks.Model;

public class EditorModel
{
    public List<EditorRowModel> Rows { get; set; }

    // Should these two even be public? They're config metadata that could be internal, either direct or via getter function
    // TODO: Create separate type for this
    [JsonIgnore]
    public List<EditorRowModel> ColumnDefinitions { get; }


    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public EditorModel() : this(new List<BlockRegistration>(), new List<EditorRowModel>()) { }

    public EditorModel(List<BlockRegistration> editorRegistrations, List<EditorRowModel>? columnDefinitions = null)
    {
        ColumnDefinitions = columnDefinitions ?? new List<EditorRowModel>();

        Rows = new List<EditorRowModel>();

        AddDefaultColumnDefinitions();

        _jsonSerializerOptions = new JsonSerializerOptions() { TypeInfoResolver = new JsonSerializerTypeResolver(BlockRegistrationService.RegisteredBlocks) };
    }

    public string GetHTML()
    {
        var sb = new StringBuilder();
        foreach (var row in Rows)
        {
            sb.Append(row.Render());
        }
        return sb.ToString();
    }

    public string GetJson()
    {
        return JsonSerializer.Serialize(this, _jsonSerializerOptions);
    }

    public void Load(string data)
    {
        var model = JsonSerializer.Deserialize<EditorModel>(data, _jsonSerializerOptions);
        Rows = model?.Rows ?? new List<EditorRowModel>();
    }
}
