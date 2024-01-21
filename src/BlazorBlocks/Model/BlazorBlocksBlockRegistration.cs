namespace BlazorBlocks.Model;

public record BlazorBlocksBlockRegistration
{
    public string Name { get; }

    public string? Image { get; }

    public Type BlockModel { get; }
    public Type EditorBlock { get; }

    public BlazorBlocksBlockRegistration(string name, string? image, Type modelType, Type editorType)
    {
        if (!modelType.IsSubclassOf(typeof(BlazorBlocksEditorBlockBaseModel)))
        {
            throw new ArgumentException($"{nameof(modelType)} is not derived from {nameof(BlazorBlocksEditorBlockBaseModel)}.");
        }

        Name = name;
        Image = image;
        BlockModel = modelType;
        EditorBlock = editorType;
    }
}
