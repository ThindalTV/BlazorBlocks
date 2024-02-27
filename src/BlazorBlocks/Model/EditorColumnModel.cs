using System;
using System.Text;
using System.Text.Json.Serialization;

namespace BlazorBlocks.Model;

public class EditorColumnModel
{
    // TODO: Replace this with column classname
    public string? ColumnClass { get; set; }

    public List<BlazorBlocksEditorBlockBaseModel> Blocks { get; set; }

    public EditorColumnModel()
    {
        Blocks = new List<BlazorBlocksEditorBlockBaseModel>();
    }

    public string Render()
    {
        var sb = new StringBuilder();
        sb.Append($"""<div class="{ColumnClass}">""");
        foreach (var block in Blocks)
        {
            sb.Append(block.Render());
        }
        sb.Append("</div>");
        return sb.ToString();
    }

    public void MoveBlock(BlazorBlocksEditorBlockBaseModel block, int newPosition)
    {
        Blocks.Remove(block);
        Blocks.Insert(newPosition, block);
    }

    public void AddBlock(BlazorBlocksEditorBlockBaseModel block, int index)
    {
        Blocks.Add(block);
    }

    public void RemoveBlock(BlazorBlocksEditorBlockBaseModel block)
    {
        Blocks.Remove(block);
    }
}
