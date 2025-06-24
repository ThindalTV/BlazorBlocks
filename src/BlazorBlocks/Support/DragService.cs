using BlazorBlocks.Model;

namespace BlazorBlocks.Support;
internal class DragService
{
    private EditorBlockModel? _draggedBlock;
    public Action<bool>? DraggedBlockChanged { get; set; }
    public EditorBlockModel? DraggedBlock
    {
        get => _draggedBlock;
        set { _draggedBlock = value; DraggedBlockChanged?.Invoke(value != null); }
    }

    private EditorRowModel? _draggedRow;
    public Action<bool>? DragRowChanged { get; set; }

    public EditorRowModel? DraggedRow
    {
        get => _draggedRow;
        set { _draggedRow = value; DragRowChanged?.Invoke(value != null); }
    }

    public EditorModel? DraggedObjectParent { get; set; }
}
