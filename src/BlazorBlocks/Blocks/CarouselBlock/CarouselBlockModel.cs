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
        return $"{RenderStyles()}{RenderCarousel()}";
    }

    private static string RenderStyles() => """
        <style>
            .carousel { max-height: 100px; position: relative; width: 100%; }
            .carousel .item:nth-child(n+2) { display: none; }
        </style>
        """;

    private string RenderCarousel()
    {
        var itemsHtml = string.Join("", Items.Select(RenderItem));
        return $"<div class=\"carousel\">{itemsHtml}</div>";
    }

    private static string RenderItem(CarouselItem item) => $$"""
        <div class="item">
            <img src="{{item.ImageUrl}}" alt="{{item.AltText}}" title="{{item.Caption}}" />
            <div class="caption" style="position: absolute; left: 50px; bottom: 50px;">{{item.Caption}}</div>
            <div class="description" style="position: absolute; left: 50px; bottom: 25px;">{{item.Description}}</div>
        </div>
        """;
}

public class CarouselItem
{
    public string? ImageUrl { get; set; }
    public string? AltText { get; set; }
    public string? Caption { get; set; }
    public string? Description { get; set; }
}
