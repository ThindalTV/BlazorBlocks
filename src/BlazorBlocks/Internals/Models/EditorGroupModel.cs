using System.Text;

namespace BlazorBlocks.Internals.Models;

public class EditorGroupModel : BaseModel
{
    
    public List<EditorColumnModel> Columns { get; set; } = [];

    public string GroupTypeName { get; set; } = "Unset";

    public string CssClass { get; set; } = "row";

    public async Task<string> RenderAsync(BlockHtmlRenderer renderer)
    {
        var sb = new StringBuilder();
        sb.Append($"<div class=\"{CssHelper.SanitizeCssClass(CssClass)}\">");
        foreach (var column in Columns)
        {
            sb.Append(await column.RenderAsync(renderer));
        }
        sb.Append("</div>");
        return sb.ToString();
    }
}
