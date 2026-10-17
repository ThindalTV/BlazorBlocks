using BlazorBlocks.Customization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace BlazorBlocks;

/// <summary>
/// Renders block renderer components to HTML strings.
/// Registered by <c>AddBlazorBlocks()</c>; use <see cref="CreateStandalone"/> outside of dependency injection (for example in tests).
/// </summary>
public sealed class BlockHtmlRenderer : IAsyncDisposable
{
    private readonly HtmlRenderer _renderer;

    public BlockHtmlRenderer(IServiceProvider services, ILoggerFactory? loggerFactory = null)
    {
        _renderer = new HtmlRenderer(services, loggerFactory ?? NullLoggerFactory.Instance);
    }

    /// <summary>Creates a renderer with an empty service provider and no logging.</summary>
    public static BlockHtmlRenderer CreateStandalone() =>
        new(new ServiceCollection().BuildServiceProvider());

    /// <summary>Renders the renderer component registered for the model's type.</summary>
    public Task<string> RenderBlockAsync(BaseBlockModel model)
    {
        var rendererType = BlockRegistrationService.Blocks
            .FirstOrDefault(r => r.BlockModel == model.GetType())?.RendererBlock
            ?? throw new InvalidOperationException(
                $"No block is registered for model type '{model.GetType().Name}', so it has no renderer.");

        return RenderAsync(rendererType, new Dictionary<string, object?> { ["Model"] = model });
    }

    /// <summary>Renders a specific renderer component for the given model.</summary>
    public Task<string> RenderAsync<TRenderer, TModel>(TModel model)
        where TModel : BaseBlockModel
        where TRenderer : BlockRenderer<TModel> =>
        RenderAsync(typeof(TRenderer), new Dictionary<string, object?> { ["Model"] = model });

    private Task<string> RenderAsync(Type componentType, IDictionary<string, object?> parameters) =>
        _renderer.Dispatcher.InvokeAsync(async () =>
        {
            var output = await _renderer.RenderComponentAsync(componentType, ParameterView.FromDictionary(parameters));
            return output.ToHtmlString();
        });

    public ValueTask DisposeAsync()
    {
        _renderer.Dispose();
        return ValueTask.CompletedTask;
    }
}
