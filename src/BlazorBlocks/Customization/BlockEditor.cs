using Microsoft.AspNetCore.Components;

namespace BlazorBlocks.Customization;

public class BlockEditor<T> : ComponentBase where T : BaseBlockModel
{
    [CascadingParameter]
    public required Editor Editor { get; set; }

    [Parameter, EditorRequired]
    public required T Model { get; set; }

    protected async Task OnValuesChanged()
    {
        await Editor.OnModelUpdated.InvokeAsync();

        await Model.OnValuesChanged();
    }
}
