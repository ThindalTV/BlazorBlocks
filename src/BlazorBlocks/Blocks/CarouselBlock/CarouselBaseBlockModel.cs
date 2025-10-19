using BlazorBlocks.Customization;
using System.Text;

namespace BlazorBlocks.Blocks.CarouselBlock;
public class CarouselBaseBlockModel : BaseBlockModel
{
    public override string EditorName => "Carousel";

    public List<CarouselItem> Items { get; set; } = [];

    public CarouselBaseBlockModel() : base(typeof(CarouselBlockEditor))
    {
    }

    public override string Render()
    {
        var css = $$"""<style>.carousel { max-height: 100px; position: relative; width: 100%; } .carousel .item:nth-child(n+2) { display: none; }</style>""";
        var output = new StringBuilder();
        output.Append($"""{css}<div class="carousel">""");
        foreach(var item in Items)
        {
            output.Append($"""<div class="item">""");
            output.Append($"""<img src="{item.ImageUrl}" alt={item.AltText}" title={item.Caption}" />""");
            output.Append($"""<div class="caption" style="position: absolute; left: 50px; bottom: 50px;">{item.Caption}</div>""");
            output.Append($"""<div class="description" style="position: absolute; left: 50px; bottom: 25px;">{item.Description}</div></div>""");
        }
        output.Append("</div>");
        return output.ToString();
    }
}

public class CarouselItem
{
    public string? ImageUrl { get; set; }
    public string? AltText { get; set; }
    public string? Caption { get; set; }
    public string? Description { get; set; }
}
