using BlazorBlocks.BlockEditor.Internals;
using BlazorBlocks.Model;

namespace BlazorBlocks.Support;
internal class DragService
{
    private BlazorBlocksEditorBlockBaseModel? _draggedBlock;
    public Func<bool, Task>? DraggedBlockChanged { get; set; }
    public BlazorBlocksEditorBlockBaseModel? DraggedBlock
    {
        get => _draggedBlock;
        set { _draggedBlock = value; DraggedBlockChanged?.Invoke(value != null); }
    }

    private EditorRowModel? _draggedRow;
    public Func<bool, Task>? DragRowChanged { get; set; }

    public EditorRowModel? DraggedRow
    {
        get => _draggedRow;
        set { _draggedRow = value; DragRowChanged?.Invoke(value != null); }
    }
}
