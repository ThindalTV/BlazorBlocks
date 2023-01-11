using System.Text;

namespace BlazorBlocks.Shared.BlockEditor.Model;

public class EditorModel
{
    public List<EditorRowModel> Rows { get; set; }
	public EditorModel()
	{
        Rows = new List<EditorRowModel>();
    }

    public string Render()
    {
        var sb = new StringBuilder();
        foreach (var row in Rows)
        {
            sb.Append(row.Render());
        }
        return sb.ToString();
    }
}
