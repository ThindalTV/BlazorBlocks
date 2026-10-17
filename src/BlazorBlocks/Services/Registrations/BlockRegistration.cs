using BlazorBlocks.Customization;

namespace BlazorBlocks.Services.Registrations;

abstract record BlockRegistration
{
    public abstract string Name { get; }
    public abstract string? Image { get; }
    public abstract Type BlockModel { get; }
    public abstract Type EditorBlock { get; }
    public abstract Type RendererBlock { get; }
}

record BlockRegistration<TModel, TEditor, TRenderer> : BlockRegistration
    where TModel : BaseBlockModel
    where TEditor : BlockEditor<TModel>
    where TRenderer : BlockRenderer<TModel>
{
    public override string Name { get; }

    public override string? Image { get; }

    public override Type BlockModel => typeof(TModel);
    public override Type EditorBlock => typeof(TEditor);
    public override Type RendererBlock => typeof(TRenderer);

    public BlockRegistration(string name, string? image = null)
    {
        Name = name;
        Image = image;
    }
}
