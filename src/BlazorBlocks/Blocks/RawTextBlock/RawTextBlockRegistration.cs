using BlazorBlocks.Services.Registrations;

namespace BlazorBlocks.Blocks.RawTextBlock;

public record RawTextBlockRegistration()
    : BlockRegistration("Raw text", "https://icon-sets.iconify.design/logo-iconify.svg", typeof(RawTextBlockModel), typeof(RawTextEditorBlock));