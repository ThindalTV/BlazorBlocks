namespace BlazorBlocks.Model;
public record RowRegistration
{
    public string Name { get; }

    // TODO: Add row cssclass to remove depenency on bootstrap. Currently hardcoded to "row" in EditorRowModel.Render()

    public IReadOnlyList<ColumnRegistration> Columns { get; }

    public RowRegistration(string name, IReadOnlyList<ColumnRegistration> columns)
    {
        Name = name;
        Columns = columns;
    }
}
