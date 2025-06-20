using BlazorBlocks.Model.Registrations;

namespace BlazorBlocks.Component.Dialogs;

public record NewRowDialogResult(RowRegistration RowType, int InsertIndex);