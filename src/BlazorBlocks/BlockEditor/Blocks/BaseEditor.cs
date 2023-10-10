using BlazorBlocks.Shared.BlockEditor.Model;
using Microsoft.AspNetCore.Components;

namespace BlazorBlocks.Shared.BlockEditor.Blocks;

public class BaseEditor<T> : ComponentBase
{
    [Parameter]
    public T? Model { get; set; }
}
