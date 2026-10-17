using BlazorBlocks.Customization;

namespace BlazorBlocks.Blocks.ImageBlock;

public class ImageBlockModel : BaseBlockModel
{
    public override string EditorName => "Image";

    public string? ImageUrl { get; set; }
    public string? AltText { get; set; } = string.Empty;

    public int? Height;
    public ImageBlockModel() : base(typeof(ImageBlockEditor))
    {
    }
}
