using BlazorBlocks.Model;
using BlazorBlocks.Support;
using Microsoft.AspNetCore.Components;

namespace BlazorBlocks.BlockEditor.Internals;

public partial class AddRow
{
    private bool _beingDragged;
    private bool _beingHoveredOver;

    private EditorModel _draggedModel;
    private DragObjectType _draggedObjectType;

    private async Task DragChanged(bool dragging, DragObjectType objectTypeBeingDragged)
    {
        if (ObjectType == objectTypeBeingDragged)
        {
            _beingDragged = dragging;
            _draggedObjectType = objectTypeBeingDragged;
            _draggedModel = DragService.DraggedRow;
            await InvokeAsync(StateHasChanged);
        }
    }

    protected override void OnInitialized()
    {
        DragService.DragRowChanged += (dragging) => DragChanged(dragging, DragObjectType.Row);
        DragService.DraggedBlockChanged += (dragging) => DragChanged(dragging, DragObjectType.Block);
        base.OnInitialized();
    }

    [Parameter, EditorRequired] public required int Index { get; set; }
    [Parameter, EditorRequired] public required DragObjectType ObjectType { get; set; }
    [Parameter] public EventCallback<int> OnClicked { get; set; }
    [Parameter] public EventCallback<ObjectDroppedResult> OnDropped { get; set; }

    private void AddClicked()
    {
        if (OnClicked.HasDelegate)
        {
            OnClicked.InvokeAsync(Index);
        }
    }

    private bool CanDrop() => ObjectType == _draggedObjectType;

    private void DropEntered()
    {
        if (CanDrop())
        {
            _beingHoveredOver = true;
        }
    }

    private void DropExited()
    {
        _beingHoveredOver = false;
    }

    private async Task ObjectDropped()
    {
        if (CanDrop())
        {
            _beingHoveredOver = false;
            var result = 
                new ObjectDroppedResult(
                    Index
                    , _draggedObjectType == DragObjectType.Row ? null : 
                    ,_draggedModel);

            if (OnDropped.HasDelegate)
            {
                await OnDropped.InvokeAsync(result);
            }
        }
    }
}