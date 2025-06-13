using BlazorBlocks.Model;

namespace BlazorBlocks.BlockEditor.Internals;

public record ObjectDroppedResult(int Index, EditorColumnModel ParentColumn, EditorModel Model);