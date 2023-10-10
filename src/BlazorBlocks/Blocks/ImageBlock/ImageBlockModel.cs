using BlazorBlocks.Model;

namespace BlazorBlocks.Blocks.ImageBlock;

public class ImageBlockModel : EditorBlockBaseModel
{
    public override string EditorName => "Image";

    public string ImageUrl { get; set; } = string.Empty;
    public string AltText { get; set; } = string.Empty;
    public ImageBlockModel() : base(typeof(ImageEditorBlock))
    {
    }

    public override string Render()
    {
        if (string.IsNullOrEmpty(ImageUrl))
        {
            return "No image";
        }
        return $"""<img src="{ImageUrl}" alt="{AltText}" /><br />""";
    }

}   
