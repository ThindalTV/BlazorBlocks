using BlazorBlocks.Shared.BlockEditor.Model;
using System.Text.Json.Serialization;

namespace BlazorBlocks.Shared.BlockEditor.Blocks.ImageBlock;

public class ImageBlockModel : EditorBlockBaseModel
{
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
        return $"""<img src="{ImageUrl}" alt="{AltText}" />""";
    }

}
