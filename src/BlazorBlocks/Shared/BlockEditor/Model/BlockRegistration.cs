namespace BlazorBlocks.Shared.BlockEditor.Model;

public class BlockRegistration
{
    public string Name { get; }
    public Type BlockModel { get; }
    public Type EditorBlock { get; }

    public BlockRegistration(string name, Type modelType, Type editorType)
    {
        if (!modelType.IsSubclassOf(typeof(EditorBlockBaseModel)))
        {
            throw new ArgumentException($"{nameof(modelType)} is not derived from {nameof(EditorBlockBaseModel)}.");
        }

        Name = name;
        BlockModel = modelType;
        EditorBlock = editorType;
    }
}
