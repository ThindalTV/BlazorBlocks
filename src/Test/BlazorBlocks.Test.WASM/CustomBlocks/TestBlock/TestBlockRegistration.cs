using BlazorBlocks.Services.Registrations;

namespace BlazorBlocks.Test.WASM.CustomBlocks.TestBlock;

public record TestBlockRegistration()
    : BlockRegistration("Test BaseBlock", null, typeof(TestBaseBlockModel), typeof(TestBlockEditor));
