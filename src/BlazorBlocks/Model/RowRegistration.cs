namespace BlazorBlocks.Model;
public record RowRegistration
{
    public string Name { get; }

    public IReadOnlyList<ColumnRegistration> Columns { get; }

    public RowRegistration(string name, IReadOnlyList<ColumnRegistration> columns)
    {
        Name = name;
        Columns = columns;
    }
}
