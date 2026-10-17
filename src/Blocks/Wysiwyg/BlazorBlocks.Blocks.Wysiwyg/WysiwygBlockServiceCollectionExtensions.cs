using Microsoft.Extensions.DependencyInjection;

namespace BlazorBlocks.Blocks.Wysiwyg;

public static class WysiwygBlockServiceCollectionExtensions
{
    /// <summary>Registers the rich text (TinyMCE) block. Call after <c>AddBlazorBlocks()</c>.</summary>
    public static IServiceCollection AddWysiwygBlock(this IServiceCollection services, Action<WysiwygBlockOptions>? configure = null)
    {
        var options = new WysiwygBlockOptions();
        configure?.Invoke(options);
        services.AddSingleton(options);

        return services.AddBlazorBlock<WysiwygBlockModel, WysiwygBlockEditor, WysiwygBlockRenderer>("Rich text");
    }
}
