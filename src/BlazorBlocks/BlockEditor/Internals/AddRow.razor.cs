using BlazorBlocks.Model;
using BlazorBlocks.Support;
using Microsoft.AspNetCore.Components;

namespace BlazorBlocks.BlockEditor.Internals;

public partial class AddRow
{
    private bool _beingDragged;
    private bool _beingHoveredOver = false;

    private EditorModel? _draggedModel;
    private DragObjectType _draggedObjectType;
    private string _cssClasses;

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
                DragObjectType.Row => DukaDragService.DraggedRow,
                DragObjectType.Block => DukaDragService.DraggedBlock,
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
        DukaDragService.DragRowChanged += (dragging) => DragChanged(dragging, DragObjectType.Row);
        DukaDragService.DraggedBlockChanged += (dragging) => DragChanged(dragging, DragObjectType.Block);
        _cssClasses = ObjectType switch
        {
            DragObjectType.Row => "center-row",
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
        if (CanDrop())
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