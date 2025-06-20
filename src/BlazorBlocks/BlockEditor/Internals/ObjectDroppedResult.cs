using BlazorBlocks.Model;

namespace BlazorBlocks.BlockEditor.Internals;

public record ObjectDroppedResult(int Index, EditorModel? ParentColumn, EditorModel Model);