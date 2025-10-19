using BlazorBlocks.Customization;

namespace BlazorBlocks.Blocks.ImageBlock;

public class ImageBaseBlockModel : BaseBlockModel
{
    public override string EditorName => "Image";

    public string ImageUrl { get; set; } = string.Empty;
    public string AltText { get; set; } = string.Empty;

    public int? Height;
    public ImageBaseBlockModel() : base(typeof(ImageBlockEditor))
    {
    }

    public override string Render()
    {
        if (string.IsNullOrEmpty(ImageUrl))
        {
            return "No image";
        }

        string height = String.Empty;
        if (Height is not null)
        {
            height = $"Height=\"{Height}\" ";
        }
        return $"""<img src="{ImageUrl}" alt="{AltText}" {height}/><br />""";
    }

}   
