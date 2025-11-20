using BlazorBlocks.Services.Registrations;

namespace BlazorBlocks.Sample.WASM.CustomBlocks.SampleBlock;

public record SampleBlockRegistration()
    : BlockRegistration("Sample Block", null, typeof(SampleBlockModel), typeof(SampleBlockEditor));
