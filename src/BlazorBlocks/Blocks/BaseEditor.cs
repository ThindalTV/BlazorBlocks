using BlazorBlocks.BlockEditor;
using Microsoft.AspNetCore.Components;

namespace BlazorBlocks.Blocks;

public class BaseEditor<T> : ComponentBase
{
    [CascadingParameter]
    public required BlazorBlocksEditor Editor { get; set; }

    [Parameter, EditorRequired]
    public required T Model { get; set; }

    protected async Task OnValuesChanged()
    {
        await Editor.OnModelUpdated.InvokeAsync();
    }
}
