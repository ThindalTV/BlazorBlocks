using BlazorBlocks.BlockEditor.Internals;
using BlazorBlocks.Model;

namespace BlazorBlocks.Support;
internal class DragService
{
    private BlazorBlocksEditorBlockBaseModel? _draggedBlock;
    public Func<bool, Task>? DragObjectChanged { get; set; }
    public BlazorBlocksEditorBlockBaseModel? DraggedBlock
    {
        get => _draggedBlock;
        set { _draggedBlock = value; DragObjectChanged?.Invoke(value != null); }
    }

    private EditorRow? _draggedRow;
    public Func<bool, Task>? DragRowChanged { get; set; }

    public EditorRow? DraggedRow
    {
        get => _draggedRow;
        set { _draggedRow = value; DragRowChanged?.Invoke(value != null); }
    }
}
