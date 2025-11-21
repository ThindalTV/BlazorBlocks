using BlazorBlocks.Customization;
using System.Text;

namespace BlazorBlocks.Blocks.CarouselBlock;
public class CarouselBlockModel : BaseBlockModel
{
    public override string EditorName => "Carousel";

    public List<CarouselItem> Items { get; set; } = [];

    public CarouselBlockModel() : base(typeof(CarouselBlockEditor))
    {
    }

    public override string Render()
    {
        return RenderCarousel();
    }

    private string RenderCarousel()
    {
        var itemsHtml = string.Join("", Items.Select(RenderItem));
        return $"<div class=\"bb-carousel\">{itemsHtml}</div>";
    }

    private static string RenderItem(CarouselItem item) => item.ImageUrl is not null ? $$"""
        <div class="bb-carousel__item">
            <img src="{{item.ImageUrl}}" alt="{{item.AltText}}" title="{{item.Caption}}" />
            <div class="bb-carousel__item-caption">{{item.Caption}}</div>
            <div class="bb-carousel__item-description">{{item.Description}}</div>
        </div>
        """ : String.Empty;
}

public class CarouselItem
{
    public string? ImageUrl { get; set; }
    public string? AltText { get; set; }
    public string? Caption { get; set; }
    public string? Description { get; set; }
}
