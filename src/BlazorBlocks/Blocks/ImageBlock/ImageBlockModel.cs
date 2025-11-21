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

    public override string Render()
    {
        if (string.IsNullOrEmpty(ImageUrl))
        {
            return RenderNoImage();
        }

        return RenderImage();
    }

    private static string RenderNoImage() => "No image";

    private string RenderImage()
    {
        var heightAttribute = GetHeightAttribute();
        return $"""<img src="{ImageUrl}" alt="{AltText}" {heightAttribute}/><br />""";
    }

    private string GetHeightAttribute() =>
        Height.HasValue ? $"Height=\"{Height}\" " : string.Empty;
}
