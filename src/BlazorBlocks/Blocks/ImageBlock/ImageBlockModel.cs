using System.Text.Json.Serialization;
using BlazorBlocks.Customization;

namespace BlazorBlocks.Blocks.ImageBlock;

/// <summary>Where an <see cref="ImageBlockModel"/> gets its image from.</summary>
public enum ImageSourceMode
{
    /// <summary>The image is loaded from <see cref="ImageBlockModel.ImageUrl"/>.</summary>
    Url = 0,

    /// <summary>The image is stored inside the block as base64 and rendered as a <c>data:</c> URI.</summary>
    Embedded = 1,
}

public class ImageBlockModel : BaseBlockModel
{
    /// <summary>Images larger than this are rejected by the editor, since they are stored in the saved JSON.</summary>
    public const long MaxEmbeddedBytes = 1024 * 1024;

    private static readonly HashSet<string> EmbeddableTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/png", "image/jpeg", "image/gif", "image/webp", "image/svg+xml",
    };

    public override string EditorName => "Image";

    public ImageSourceMode Source { get; set; } = ImageSourceMode.Url;
    public string? ImageUrl { get; set; }

    /// <summary>The image bytes as base64. Only used when <see cref="Source"/> is <see cref="ImageSourceMode.Embedded"/>.</summary>
    public string? EmbeddedData { get; set; }

    /// <summary>The MIME type of <see cref="EmbeddedData"/>, for example <c>image/png</c>.</summary>
    public string? EmbeddedContentType { get; set; }

    public string? AltText { get; set; } = string.Empty;

    public int? Height;

    /// <summary>The value for the <c>src</c> attribute, or null when the block has no usable image.</summary>
    [JsonIgnore]
    public string? ResolvedSource => Source == ImageSourceMode.Embedded
        ? ToDataUri(EmbeddedContentType, EmbeddedData)
        : (string.IsNullOrEmpty(ImageUrl) ? null : ImageUrl);

    public ImageBlockModel() : base(typeof(ImageBlockEditor))
    {
    }

    public static bool IsEmbeddableType(string? contentType) =>
        contentType != null && EmbeddableTypes.Contains(contentType);

    /// <summary>Builds a <c>data:</c> URI, or returns null when the type is not an allowed image type or there is no data.</summary>
    public static string? ToDataUri(string? contentType, string? base64) =>
        IsEmbeddableType(contentType) && !string.IsNullOrEmpty(base64)
            ? $"data:{contentType!.ToLowerInvariant()};base64,{base64}"
            : null;

    /// <summary>Stores the bytes of an uploaded image and switches the block to <see cref="ImageSourceMode.Embedded"/>.</summary>
    public void SetEmbeddedImage(string contentType, ReadOnlySpan<byte> bytes)
    {
        if (!IsEmbeddableType(contentType))
        {
            throw new ArgumentException($"'{contentType}' is not a supported image type.", nameof(contentType));
        }

        if (bytes.Length > MaxEmbeddedBytes)
        {
            throw new ArgumentException($"The image is larger than {MaxEmbeddedBytes / 1024} KB.", nameof(bytes));
        }

        EmbeddedContentType = contentType.ToLowerInvariant();
        EmbeddedData = Convert.ToBase64String(bytes);
        Source = ImageSourceMode.Embedded;
    }
}
