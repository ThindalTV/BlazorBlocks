namespace BlazorBlocks.Shared.BlockEditor.Model;

public class BlockRegistration
{
    public string Name { get; }

    public string? Image { get; }

    public Type BlockModel { get; }
    public Type EditorBlock { get; }

    public BlockRegistration(string name, string? image, Type modelType, Type editorType)
    {
        if (!modelType.IsSubclassOf(typeof(EditorBlockBaseModel)))
        {
            throw new ArgumentException($"{nameof(modelType)} is not derived from {nameof(EditorBlockBaseModel)}.");
        }

        Name = name;
        Image = image;
        BlockModel = modelType;
        EditorBlock = editorType;
    }
}
