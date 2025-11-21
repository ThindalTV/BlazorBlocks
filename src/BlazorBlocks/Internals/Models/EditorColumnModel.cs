using System.Text;

namespace BlazorBlocks.Internals.Models;

public class EditorColumnModel : BaseModel
{
    // TODO: Replace this with column classname
    public string? ColumnClass { get; set; }

    public List<EditorBlockModel> Blocks { get; set; } = [];

    public string Render()
    {
        var sb = new StringBuilder();
        sb.Append($"""<div class="{ColumnClass}">""");
        foreach (var block in Blocks)
        {
            sb.Append(block.Block.Render());
        }
        sb.Append("</div>");
        return sb.ToString();
    }

    public void AddBlock(EditorBlockModel block, int index)
    {
        Blocks.Insert(index, block);
    }

    public void RemoveBlock(EditorBlockModel block)
    {
        Blocks.Remove(block);
    }
}