using System;
using System.Text;
using System.Text.Json.Serialization;

namespace BlazorBlocks.Model;

public class EditorColumnModel
{
    // TODO: Replace this with column classname
    public string? ColumnSize { get; set; }

    [JsonIgnore]
    public Action OnUpdated { get; set; }

    public List<BlazorBlocksEditorBlockBaseModel> Blocks { get; set; }

    public EditorColumnModel()
    {
        Blocks = new List<BlazorBlocksEditorBlockBaseModel>();
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

    public void MoveBlock(BlazorBlocksEditorBlockBaseModel block, int newPosition)
    {
        Blocks.Remove(block);
        Blocks.Insert(newPosition, block);
    }

    public void AddBlock(BlazorBlocksEditorBlockBaseModel block, int index)
    {
        block.OnUpdated = () =>
        {
            OnUpdated?.Invoke();
        };
        Blocks.Add(block);
    }

    public void RemoveBlock(BlazorBlocksEditorBlockBaseModel block)
    {
        block.OnUpdated = null;
        Blocks.Remove(block);
    }
}
