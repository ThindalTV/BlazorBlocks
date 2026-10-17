using BlazorBlocks.Customization;

namespace BlazorBlocks.Blocks.CarouselBlock;
public class CarouselBlockModel : BaseBlockModel
{
    public override string EditorName => "Carousel";

    public List<CarouselItem> Items { get; set; } = [];

    public CarouselBlockModel() : base(typeof(CarouselBlockEditor))
    {
    }
}

public class CarouselItem
{
    public string? ImageUrl { get; set; }
    public string? AltText { get; set; }
    public string? Caption { get; set; }
    public string? Description { get; set; }
}
