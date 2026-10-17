namespace BlazorBlocks.Blocks.Wysiwyg;

public class WysiwygBlockOptions
{
    /// <summary>
    /// Where TinyMCE is loaded from. Defaults to the jsDelivr copy of the GPL build; point this at a self-hosted
    /// <c>tinymce.min.js</c> (skins, plugins and icons are resolved next to it) to avoid the CDN.
    /// </summary>
    public string ScriptUrl { get; set; } = "https://cdn.jsdelivr.net/npm/tinymce@7.9.1/tinymce.min.js";

    /// <summary>The TinyMCE <c>license_key</c>. "gpl" selects the open-source license; use your own key for a commercial one.</summary>
    public string LicenseKey { get; set; } = "gpl";

    /// <summary>Height of the editing area in pixels.</summary>
    public int Height { get; set; } = 320;
}
