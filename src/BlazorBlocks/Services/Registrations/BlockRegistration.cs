using BlazorBlocks.Customization;

namespace BlazorBlocks.Services.Registrations;

public abstract record BlockRegistration
{
    public string Name { get; }

    public string? Image { get; }

    public Type BlockModel { get; }
    public Type EditorBlock { get; }

    public BlockRegistration(string name, string? image, Type modelType, Type editorType)
    {
        if (!modelType.IsSubclassOf(typeof(BaseBlockModel)))
        {
            throw new ArgumentException($"{nameof(modelType)} is not derived from {nameof(BaseBlockModel)}.");
        }

        Name = name;
        Image = image;
        BlockModel = modelType;
        EditorBlock = editorType;
    }
}
