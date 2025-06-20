namespace BlazorBlocks.Model;

public class EditorModel
{
    public Guid Id { get; } = Guid.NewGuid();

    public EditorModel? ParentModel {get; set; }
}