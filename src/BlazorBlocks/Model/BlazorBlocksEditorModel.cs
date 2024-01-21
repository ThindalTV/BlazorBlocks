using System.Text;
using System.Text.Json;

namespace BlazorBlocks.Model;

public class EditorModel
{
    public List<EditorRowModel> Rows { get; set; }

    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public EditorModel()
    {
        Rows = new List<EditorRowModel>();

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
