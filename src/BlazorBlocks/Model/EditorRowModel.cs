using System.Text;

namespace BlazorBlocks.Model;

public class EditorRowModel : EditorModel
{
    
    public List<EditorColumnModel> Columns { get; set; }

    public string RowTypeName { get; set; }

    public EditorRowModel()
    {
        Columns = new List<EditorColumnModel>();
        RowTypeName = "Blergh";
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
