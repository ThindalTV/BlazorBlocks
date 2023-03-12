using BlazorBlocks.Shared.BlockEditor.Blocks.ImageBlock;
using BlazorBlocks.Shared.BlockEditor.Blocks.RawTextBlock;
using BlazorBlocks.Shared.BlockEditor.Blocks.TitleBlock;
using System.Text;
using System.Text.Json;

namespace BlazorBlocks.Shared.BlockEditor.Model;

public class EditorModel
{
    public List<EditorRowModel> Rows { get; set; }

    public List<EditorRowModel> ColumnDefinitions { get; set; }
    public List<BlockRegistration> BlockRegistrations { get; }

    private readonly JsonSerializerTypeResolver _jsonSerializerResolver;

    public EditorModel() : this(new List<BlockRegistration>())
    {

    }

    public EditorModel(List<BlockRegistration> editorRegistrations)
    {
        BlockRegistrations = editorRegistrations ?? new List<BlockRegistration>();

        ColumnDefinitions = new List<EditorRowModel>();

        AddDefaultColumnDefinitions();

        BlockRegistrations.Add(new BlockRegistration("Title", null, typeof(TitleBlockModel), typeof(TitleEditorBlock)));
        BlockRegistrations.Add(new BlockRegistration("Raw text", "https://icon-sets.iconify.design/logo-iconify.svg", typeof(RawTextBlockModel), typeof(RawTextEditorBlock)));
        BlockRegistrations.Add(new BlockRegistration("Image", null, typeof(ImageBlockModel), typeof(ImageEditorBlock)));

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
    void AddDefaultColumnDefinitions()
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
    }
}
