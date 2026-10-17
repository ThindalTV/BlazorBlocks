using BlazorBlocks.Blocks.ImageBlock;
using BlazorBlocks.Blocks.SvgBlock;

namespace BlazorBlocks.Tests.Model;

/// <summary>Covers the embedded (base64) image mode and the SVG block.</summary>
public class EditorsTests : IAsyncLifetime
{
    private BlockHtmlRenderer _renderer = null!;

    public Task InitializeAsync()
    {
        TestRegistration.EnsureRegistered();
        _renderer = BlockHtmlRenderer.CreateStandalone();
        return Task.CompletedTask;
    }

    public async Task DisposeAsync() => await _renderer.DisposeAsync();

    // ---- Embedded images ----

    [Fact]
    public async Task Image_Embedded_RendersDataUri()
    {
        var model = new ImageBlockModel { AltText = "A" };
        model.SetEmbeddedImage("image/png", [1, 2, 3]);

        Assert.Equal("""<img src="data:image/png;base64,AQID" alt="A" aria-label="A" aria-hidden="false" /><br>""",
            await _renderer.RenderBlockAsync(model));
    }

    [Fact]
    public async Task Image_Embedded_WithoutData_RendersPlaceholder() =>
        Assert.Equal("No image",
            await _renderer.RenderBlockAsync(new ImageBlockModel { Source = ImageSourceMode.Embedded, ImageUrl = "ignored.png" }));

    [Fact]
    public async Task Image_Embedded_IgnoresUrl()
    {
        var model = new ImageBlockModel { ImageUrl = "u.png" };
        model.SetEmbeddedImage("image/gif", [9]);
        Assert.Contains("data:image/gif;base64,CQ==", await _renderer.RenderBlockAsync(model));
    }

    [Fact]
    public async Task Image_UrlMode_IgnoresEmbeddedData()
    {
        var model = new ImageBlockModel { ImageUrl = "u.png", EmbeddedContentType = "image/png", EmbeddedData = "AQID" };
        Assert.Contains("""src="u.png" """, await _renderer.RenderBlockAsync(model));
    }

    [Fact]
    public async Task Image_Embedded_RejectsNonImageContentTypeAtRenderTime() =>
        Assert.Equal("No image", await _renderer.RenderBlockAsync(new ImageBlockModel
        {
            Source = ImageSourceMode.Embedded,
            EmbeddedContentType = "text/html",
            EmbeddedData = "PHNjcmlwdD4=",
        }));

    [Fact]
    public void SetEmbeddedImage_RejectsUnsupportedType() =>
        Assert.Throws<ArgumentException>(() => new ImageBlockModel().SetEmbeddedImage("application/pdf", [1]));

    [Fact]
    public void SetEmbeddedImage_RejectsOversizedImage() =>
        Assert.Throws<ArgumentException>(() =>
            new ImageBlockModel().SetEmbeddedImage("image/png", new byte[ImageBlockModel.MaxEmbeddedBytes + 1]));

    [Fact]
    public void Image_EmbeddedData_SurvivesJsonRoundTrip()
    {
        var source = new ImageBlockModel { AltText = "A" };
        source.SetEmbeddedImage("image/webp", [1, 2, 3]);
        var group = new BlazorBlocks.Internals.Models.EditorGroupModel
        {
            Columns = [new() { Blocks = [new() { Block = source }] }]
        };
        var model = new BlazorBlocksModel { Groups = [group] };

        var loaded = new BlazorBlocksModel();
        loaded.Load(model.GetJson());

        var image = Assert.IsType<ImageBlockModel>(loaded.Groups[0].Columns[0].Blocks[0].Block);
        Assert.Equal(ImageSourceMode.Embedded, image.Source);
        Assert.Equal("data:image/webp;base64,AQID", image.ResolvedSource);
    }

    // ---- SVG ----

    private const string Circle = """<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 10 10"><circle cx="5" cy="5" r="4" /></svg>""";

    [Fact]
    public async Task Svg_RendersInlineMarkup()
    {
        var html = await _renderer.RenderBlockAsync(new SvgBlockModel { Markup = Circle });

        Assert.Equal("""<div class="bb-svg" aria-hidden="true"><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 10 10"><circle cx="5" cy="5" r="4" /></svg></div>""", html);
    }

    [Fact]
    public async Task Svg_WithAlt_IsAnImage()
    {
        var html = await _renderer.RenderBlockAsync(new SvgBlockModel { Markup = Circle, AltText = "A circle" });
        Assert.StartsWith("""<div class="bb-svg" role="img" aria-label="A circle">""", html);
    }

    [Fact]
    public async Task Svg_Empty_RendersNothing() =>
        Assert.Equal("", await _renderer.RenderBlockAsync(new SvgBlockModel()));

    [Theory]
    [InlineData("not xml")]
    [InlineData("<html><body/></html>")]
    [InlineData("<svg><unclosed></svg>")]
    public async Task Svg_Invalid_RendersErrorMessage(string markup)
    {
        var html = await _renderer.RenderBlockAsync(new SvgBlockModel { Markup = markup });
        Assert.StartsWith("""<div class="bb-svg bb-svg--invalid" role="alert">Invalid SVG: """, html);
    }

    [Fact]
    public void Sanitize_RemovesScriptsAndForeignContent()
    {
        var result = SvgSanitizer.Sanitize(
            """<svg xmlns="http://www.w3.org/2000/svg"><script>alert(1)</script><foreignObject><div/></foreignObject><style>*{}</style><rect width="1" height="1"/></svg>""");

        Assert.Equal("""<svg xmlns="http://www.w3.org/2000/svg"><rect width="1" height="1" /></svg>""", result);
    }

    [Fact]
    public void Sanitize_RemovesEventHandlersAndScriptUrls()
    {
        var result = SvgSanitizer.Sanitize(
            """<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" onload="x()"><a href="javascript:alert(1)"><rect onclick="x()" width="1"/></a><a xlink:href=" JavaScript:alert(1)"/><a href="#ok"/></svg>""")!;

        Assert.DoesNotContain("onload", result);
        Assert.DoesNotContain("onclick", result);
        Assert.DoesNotContain("javascript", result, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("""href="#ok" """.TrimEnd(), result);
    }

    [Fact]
    public void Sanitize_RejectsDtd() =>
        Assert.Null(SvgSanitizer.Sanitize("""<!DOCTYPE svg [<!ENTITY x "y">]><svg xmlns="http://www.w3.org/2000/svg">&x;</svg>"""));

    [Fact]
    public void Svg_SurvivesJsonRoundTrip()
    {
        var group = new BlazorBlocks.Internals.Models.EditorGroupModel
        {
            Columns = [new() { Blocks = [new() { Block = new SvgBlockModel { Markup = Circle, AltText = "c" } }] }]
        };
        var loaded = new BlazorBlocksModel();
        loaded.Load(new BlazorBlocksModel { Groups = [group] }.GetJson());

        var svg = Assert.IsType<SvgBlockModel>(loaded.Groups[0].Columns[0].Blocks[0].Block);
        Assert.Equal(Circle, svg.Markup);
        Assert.Equal("c", svg.AltText);
    }
}
