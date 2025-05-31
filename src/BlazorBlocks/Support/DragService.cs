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

    public EditorRow? DraggedRow { get; set; }
}
