using System.Text;

namespace BlazorBlocks.Model;

public class EditorColumnModel : EditorModel
{
    // TODO: Replace this with column classname
    public string? ColumnClass { get; set; }

    public List<EditorBlockModel> BlocksModels { get; set; }

    public EditorColumnModel()
    {
        BlocksModels = new List<EditorBlockModel>();
    }

    public string Render()
    {
        var sb = new StringBuilder();
        sb.Append($"""<div class="{ColumnClass}">""");
        foreach (var block in BlocksModels)
        {
            sb.Append(block.Block.Render());
        }
        sb.Append("</div>");
        return sb.ToString();
    }

    public void MoveBlock(EditorBlockModel block, int newPosition)
    {
        BlocksModels.Remove(block);
        BlocksModels.Insert(newPosition, block);
    }

    public void AddBlock(EditorBlockModel block, int index)
    {
        BlocksModels.Insert(index, block);
    }

    public void RemoveBlock(EditorBlockModel block)
    {
        BlocksModels.Remove(block);
    }
}