using System.Text;

namespace BlazorBlocks.Internals.Models;

public class EditorGroupModel : BaseModel
{
    
    public List<EditorColumnModel> Columns { get; set; } = [];

    public string GroupTypeName { get; set; } = "Unset";

    public string CssClass { get; set; } = "row";

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
