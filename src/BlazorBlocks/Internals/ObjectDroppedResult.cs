using BlazorBlocks.Internals.Models;

namespace BlazorBlocks.Internals;

public record ObjectDroppedResult(int Index, BaseModel? ParentColumn, BaseModel? Model);