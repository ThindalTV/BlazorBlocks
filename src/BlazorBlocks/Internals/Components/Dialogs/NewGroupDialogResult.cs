using BlazorBlocks.Services.Registrations;

namespace BlazorBlocks.Internals.Components.Dialogs;

public record NewGroupDialogResult(GroupRegistration GroupType, int InsertIndex);