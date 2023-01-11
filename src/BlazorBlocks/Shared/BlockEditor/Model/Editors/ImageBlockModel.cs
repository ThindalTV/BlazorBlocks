using BlazorBlocks.Shared.BlockEditor.Blocks.Editors;

namespace BlazorBlocks.Shared.BlockEditor.Model.Editors;

public class ImageBlockModel : EditorBlockBaseModel
{
    public string ImageUrl { get; set; }
    public string AltText { get; set; }
    public ImageBlockModel()
    {
        BlazorBlockEditor = typeof(ImageEditorBlock);
    }
    public override string Render()
    {
        if (string.IsNullOrEmpty(ImageUrl))
        {
            return "No image";
        }
        return $"<img src=\"{ImageUrl}\" alt=\"{AltText}\" />";
    }

}
