using BlazorBlocks.Services.Registrations;

namespace BlazorBlocks.Blocks.ImageBlock;

public record ImageBlockRegistration()
    : BlockRegistration("Image", null, typeof(ImageBaseBlockModel), typeof(ImageBlockEditor));
