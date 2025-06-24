using BlazorBlocks.Model.Registrations;

namespace BlazorBlocks.Component.Dialogs;

public record NewGroupDialogResult(GroupRegistration GroupType, int InsertIndex);