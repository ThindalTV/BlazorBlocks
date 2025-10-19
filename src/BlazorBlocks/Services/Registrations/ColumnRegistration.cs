namespace BlazorBlocks.Services.Registrations;
public record ColumnRegistration
{
    public string ColumnClass { get; }

    public ColumnRegistration(string columnClass)
    {
        ColumnClass = columnClass;
    }
}
