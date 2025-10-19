using BlazorBlocks.Internals.Models;
using BlazorBlocks.Internals.Support;
using Microsoft.AspNetCore.Components;

namespace BlazorBlocks.Internals.Containers;

public partial class AddBlockRow
{
    private bool _beingDragged;
    private bool _beingHoveredOver = false;

    private BaseModel? _draggedModel;
    private DragObjectType _draggedObjectType;
    private string? _cssClasses;

    private bool _isDroppable = false;

    [Parameter, EditorRequired] public required int Index { get; set; }
    [Parameter, EditorRequired] public required DragObjectType ObjectType { get; set; }
    [Parameter] public EventCallback<int> OnClicked { get; set; }
    [Parameter] public EventCallback<ObjectDroppedResult> OnDropped { get; set; }


    private void DragChanged(bool dragging, DragObjectType objectTypeBeingDragged)
    {
        // This method gets called when something is being dragged. Update the object type so we can verify if it's droppable here
        _draggedObjectType = objectTypeBeingDragged;

        if (ObjectType == objectTypeBeingDragged)
        {
            _beingDragged = dragging;
            _draggedModel = _draggedObjectType switch
            {
                DragObjectType.Group => DragService.DraggedGroup,
                DragObjectType.Block => DragService.DraggedBlock,
                _ => throw new ArgumentOutOfRangeException(nameof(objectTypeBeingDragged), objectTypeBeingDragged, null)
            };

            _isDroppable = true;

        }
        else
        {
            _isDroppable = false;
        }

        if (!dragging)
        {
            _isDroppable = false;
        }

        StateHasChanged();

    }

    protected override void OnInitialized()
    {
        DragService.DraggedGroupChanged += (dragging) => DragChanged(dragging, DragObjectType.Group);
        DragService.DraggedBlockChanged += (dragging) => DragChanged(dragging, DragObjectType.Block);
        _cssClasses = ObjectType switch
        {
            DragObjectType.Group => "center-group",
            DragObjectType.Block => "center-block",
            _ => throw new ArgumentOutOfRangeException(nameof(ObjectType), ObjectType, null)
        };

        base.OnInitialized();
    }


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
        else
        {
            _beingHoveredOver = false;
        }
    }

    private void DropExited()
    {
        _beingHoveredOver = false;
    }

    private async Task ObjectDropped()
    {
        if (CanDrop() && _draggedModel is not null)
        {
            _beingHoveredOver = false;
            var result = 
                new ObjectDroppedResult(
                    Index
                    , _draggedModel.ParentModel
                    , _draggedModel);

            if (OnDropped.HasDelegate)
            {
                await OnDropped.InvokeAsync(result);
            }
        }
    }
}