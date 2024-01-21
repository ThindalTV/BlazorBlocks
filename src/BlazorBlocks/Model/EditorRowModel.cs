using System.Text;
using System.Text.Json.Serialization;

namespace BlazorBlocks.Model;

public class EditorRowModel
{
    public List<EditorColumnModel> Columns { get; set; }

    [JsonIgnore]
    public Action OnUpdated { get; set; }

    public EditorRowModel()
    {
        Columns = new List<EditorColumnModel>();
    }

    public string Render()
    {
        var sb = new StringBuilder();
        sb.Append("<div class=\"row\">");
        foreach (var column in Columns)
        {
            sb.Append(column.Render());
        }
        sb.Append("</div>");
        return sb.ToString();
    }

    public void ColumnsSet()
    {
        foreach (var column in Columns)
        {
            column.OnUpdated = () =>
            {
                OnUpdated?.Invoke();
            };
        }
    }
}
