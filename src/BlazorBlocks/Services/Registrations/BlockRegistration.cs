using BlazorBlocks.Customization;

namespace BlazorBlocks.Services.Registrations;

abstract record BlockRegistration
{
    public abstract string Name { get; }
    public abstract string? Image { get; }
    public abstract Type BlockModel { get; }
    public abstract Type EditorBlock { get; }
}

record BlockRegistration<TModel, TEditor> : BlockRegistration
    where TModel : BaseBlockModel
    where TEditor : BlockEditor<TModel>
{
    public override string Name { get; }

    public override string? Image { get; }

    public override Type BlockModel => typeof(TModel);
    public override Type EditorBlock => typeof(TEditor);

    public BlockRegistration(string name, string? image = null)
    {
        Name = name;
        Image = image;
    }
}
