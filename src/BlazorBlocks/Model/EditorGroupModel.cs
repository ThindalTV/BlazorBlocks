using System.Text;

namespace BlazorBlocks.Model;

public class EditorGroupModel : EditorModel
{
    
    public List<EditorColumnModel> Columns { get; set; }

    public string GroupTypeName { get; set; }

    public string CssClass { get; set; } = "row";

    public EditorGroupModel()
    {
        Columns = new List<EditorColumnModel>();
    }

    public string Render()
    {
        var sb = new StringBuilder();
        sb.Append($"<div class=\"{CssClass}\">");
        foreach (var column in Columns)
        {
            sb.Append(column.Render());
        }
        sb.Append("</div>");
        return sb.ToString();
    }
}
