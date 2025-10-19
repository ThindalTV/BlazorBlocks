using BlazorBlocks.Services.Registrations;

namespace BlazorBlocks.Blocks.QuoteBlock;

public record QuoteBlockRegistration()
    : BlockRegistration("Quote", null, typeof(QuoteBaseBlockModel), typeof(QuoteBlockEditor));
