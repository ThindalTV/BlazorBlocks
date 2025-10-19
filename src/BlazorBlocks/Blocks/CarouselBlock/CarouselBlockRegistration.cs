using BlazorBlocks.Services.Registrations;

namespace BlazorBlocks.Blocks.CarouselBlock;

public record CarouselBlockRegistration()
    : BlockRegistration("Carousel", null, typeof(CarouselBaseBlockModel), typeof(CarouselBlockEditor));