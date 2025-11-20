using BlazorBlocks.Services.Registrations;

namespace BlazorBlocks.Sample.WASM.CustomBlocks.TestBlock;

public record TestBlockRegistration()
    : BlockRegistration("Test BaseBlock", null, typeof(TestBaseBlockModel), typeof(TestBlockEditor));
