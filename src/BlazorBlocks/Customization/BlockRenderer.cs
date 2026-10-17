using Microsoft.AspNetCore.Components;

namespace BlazorBlocks.Customization;

/// <summary>
/// Base class for the component that produces the public display HTML of a block.
/// Renderers are plain components: they can be written as .razor files (with IntelliSense) or as .cs classes,
/// and can be unit tested with <see cref="BlockHtmlRenderer"/>.
/// </summary>
/// <typeparam name="T">The block model to render.</typeparam>
public abstract class BlockRenderer<T> : ComponentBase where T : BaseBlockModel
{
    [Parameter, EditorRequired]
    public required T Model { get; set; }
}
