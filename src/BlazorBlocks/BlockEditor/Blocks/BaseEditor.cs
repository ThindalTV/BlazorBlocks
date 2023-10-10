using Microsoft.AspNetCore.Components;

namespace BlazorBlocks.BlockEditor.Blocks;

public class BaseEditor<T> : ComponentBase
{
    [Parameter]
    public T? Model { get; set; }
}
