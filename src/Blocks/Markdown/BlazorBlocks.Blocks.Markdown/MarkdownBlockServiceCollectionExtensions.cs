using Microsoft.Extensions.DependencyInjection;

namespace BlazorBlocks.Blocks.Markdown;

public static class MarkdownBlockServiceCollectionExtensions
{
    /// <summary>Registers the Markdown block. Call after <c>AddBlazorBlocks()</c>.</summary>
    public static IServiceCollection AddMarkdownBlock(this IServiceCollection services) =>
        services.AddBlazorBlock<MarkdownBlockModel, MarkdownBlockEditor, MarkdownBlockRenderer>("Markdown");
}
