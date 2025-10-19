using BlazorBlocks.Services.Registrations;

namespace BlazorBlocks.Blocks.TitleBlock;

public record TitleBlockRegistration()
    : BlockRegistration("Title", null, typeof(TitleBaseBlockModel), typeof(TitleEditorBlock));