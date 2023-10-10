using System.Text;

namespace BlazorBlocks.BlockEditor.Model;

public class EditorRowModel
{
    // TODO: Temp
    public string ColumnCollectionName { get; set; }

    public List<EditorColumnModel> Columns { get; set; }

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
}
