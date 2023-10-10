using System.Text;

namespace BlazorBlocks.BlockEditor.Model;

public class EditorColumnModel
{
    public string? ColumnSize { get; set; }

    public List<EditorBlockBaseModel> Blocks { get; set; }

    public EditorColumnModel()
    {
        Blocks = new List<EditorBlockBaseModel>();
    }

    public string Render()
    {
        var sb = new StringBuilder();
        sb.Append($"""<div class="{ColumnSize}">""");
        foreach (var block in Blocks)
        {
            sb.Append(block.Render());
        }
        sb.Append("</div>");
        return sb.ToString();
    }
}
