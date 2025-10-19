using BlazorBlocks.Internals.Models;

namespace BlazorBlocks.Internals.Support;
internal class DragService
{
    private EditorBlockModel? _draggedBlock;
    public Action<bool>? DraggedBlockChanged { get; set; }
    public EditorBlockModel? DraggedBlock
    {
        get => _draggedBlock;
        set { _draggedBlock = value; DraggedBlockChanged?.Invoke(value != null); }
    }

    private EditorGroupModel? _draggedGroup;
    public Action<bool>? DraggedGroupChanged { get; set; }

    public EditorGroupModel? DraggedGroup
    {
        get => _draggedGroup;
        set { _draggedGroup = value; DraggedGroupChanged?.Invoke(value != null); }
    }

    public BaseModel? DraggedObjectParent { get; set; }
}
