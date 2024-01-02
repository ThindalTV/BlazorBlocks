using BlazorBlocks.Blocks.ImageBlock;
using BlazorBlocks.Blocks.TitleBlock;
using BlazorBlocks.Blocks.RawTextBlock;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorBlocks.Model;

public class EditorModel
{
    public List<EditorRowModel> Rows { get; set; }

    // Should these two even be public? They're config metadata that could be internal, either direct or via getter function
    // TODO: Create separate type for this
    [JsonIgnore]
    public List<EditorRowModel> ColumnDefinitions { get; }

    [JsonIgnore]
    public List<BlockRegistration> BlockRegistrations { get; }

    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public EditorModel() : this(new List<BlockRegistration>(), new List<EditorRowModel>()) { }

    public EditorModel(List<BlockRegistration>? editorRegistrations = null, List<EditorRowModel>? columnDefinitions = null)
    {
        BlockRegistrations = editorRegistrations ?? new List<BlockRegistration>();
        ColumnDefinitions = columnDefinitions ?? new List<EditorRowModel>();

        Rows = new List<EditorRowModel>();

        AddDefaultColumnDefinitions();
        AddDefaultBlockTypes();

        _jsonSerializerOptions = new JsonSerializerOptions() { TypeInfoResolver = new JsonSerializerTypeResolver(BlockRegistrations) };
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

    private void AddDefaultColumnDefinitions()
    {
        ColumnDefinitions.Add(new EditorRowModel()
        {
            ColumnCollectionName = "1 column",
            Columns = new List<EditorColumnModel>
            {
               new EditorColumnModel() { ColumnSize = "col-12" },
            }
        });

        ColumnDefinitions.Add(new EditorRowModel()
        {
            ColumnCollectionName = "2 columns",
            Columns = new List<EditorColumnModel>
            {
               new EditorColumnModel() { ColumnSize = "col-6" },
               new EditorColumnModel() { ColumnSize = "col-6" }
            }
        });

        ColumnDefinitions.Add(new EditorRowModel()
        {
            ColumnCollectionName = "3 columns",
            Columns = new List<EditorColumnModel>
            {
               new EditorColumnModel() { ColumnSize = "col-4" },
               new EditorColumnModel() { ColumnSize = "col-4" },
               new EditorColumnModel() { ColumnSize = "col-4" }
            }
        });

        ColumnDefinitions.Add(new EditorRowModel()
        {
            ColumnCollectionName = "1+2 columns",
            Columns = new List<EditorColumnModel>
            {
               new EditorColumnModel() { ColumnSize = "col-4" },
               new EditorColumnModel() { ColumnSize = "col-8" }
            }
        });
    }

    private void AddDefaultBlockTypes()
    {
        BlockRegistrations.Add(new BlockRegistration("Title", null, typeof(TitleBlockModel), typeof(TitleEditorBlock)));
        BlockRegistrations.Add(new BlockRegistration("Raw text", "https://icon-sets.iconify.design/logo-iconify.svg", typeof(RawTextBlockModel), typeof(RawTextEditorBlock)));
        BlockRegistrations.Add(new BlockRegistration("Image", null, typeof(ImageBlockModel), typeof(ImageEditorBlock)));
    }
}
