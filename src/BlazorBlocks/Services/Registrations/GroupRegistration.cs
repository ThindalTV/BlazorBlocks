namespace BlazorBlocks.Services.Registrations;
public record GroupRegistration
{
    public string Name { get; }

    public string CssClass { get; }

    public IReadOnlyList<ColumnRegistration> Columns { get; }

    public GroupRegistration(string name, string cssClass, IReadOnlyList<ColumnRegistration> columns)
    {
        Name = name;
        CssClass = cssClass;
        Columns = columns;
    }
}
