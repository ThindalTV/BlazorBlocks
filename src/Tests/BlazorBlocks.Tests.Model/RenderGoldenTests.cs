using BlazorBlocks.Blocks.ImageBlock;
using BlazorBlocks.Blocks.QuoteBlock;
using BlazorBlocks.Blocks.RawTextBlock;
using BlazorBlocks.Blocks.TitleBlock;
using BlazorBlocks.Customization;
using BlazorBlocks.Internals.Models;

namespace BlazorBlocks.Tests.Model;

/// <summary>
/// Pins the HTML produced by each block's renderer and by the group/column wrappers.
/// </summary>
public class RenderGoldenTests : IAsyncLifetime
{
    private BlockHtmlRenderer _renderer = null!;

    public Task InitializeAsync()
    {
        TestRegistration.EnsureRegistered();
        _renderer = BlockHtmlRenderer.CreateStandalone();
        return Task.CompletedTask;
    }

    public async Task DisposeAsync() => await _renderer.DisposeAsync();

    private Task<string> Render(BaseBlockModel model) => _renderer.RenderBlockAsync(model);

    private static string Normalize(string s) => s.Replace("\r\n", "\n");

    // ---- Quote ----

    [Fact]
    public async Task Quote_RendersBlockquote() =>
        Assert.Equal("<blockquote>abc123</blockquote>", await Render(new QuoteBlockModel { Quote = "abc123" }));

    [Fact]
    public async Task Quote_Empty() =>
        Assert.Equal("<blockquote></blockquote>", await Render(new QuoteBlockModel()));

    [Fact]
    public async Task Quote_EncodesHtml() =>
        Assert.Equal("<blockquote>&lt;b&gt;x&lt;/b&gt;</blockquote>", await Render(new QuoteBlockModel { Quote = "<b>x</b>" }));

    // ---- RawText ----

    [Fact]
    public async Task RawText_PassesContentThroughUnencoded() =>
        Assert.Equal("<div>Here's some <b>Raw</b> text</div>",
            await Render(new RawTextBlockModel { Content = "Here's some <b>Raw</b> text" }));

    [Fact]
    public async Task RawText_Empty() =>
        Assert.Equal("<div></div>", await Render(new RawTextBlockModel()));

    // ---- Title ----

    [Theory]
    [InlineData(TitleSizeEnum.Huge, "h1")]
    [InlineData(TitleSizeEnum.Big, "h2")]
    [InlineData(TitleSizeEnum.Medium, "h3")]
    [InlineData(TitleSizeEnum.Small, "h4")]
    public async Task Title_UsesTagForSize(TitleSizeEnum size, string tag) =>
        Assert.Equal($"<{tag}>Hello</{tag}>", await Render(new TitleBlockModel { Title = "Hello", TitleSize = size }));

    [Fact]
    public async Task Title_DefaultsToH1() =>
        Assert.Equal("<h1></h1>", await Render(new TitleBlockModel()));

    // ---- Image ----

    [Fact]
    public async Task Image_NoUrl_RendersPlaceholderText()
    {
        Assert.Equal("No image", await Render(new ImageBlockModel()));
        Assert.Equal("No image", await Render(new ImageBlockModel { ImageUrl = "" }));
    }

    [Fact]
    public async Task Image_WithAlt_HasAriaLabel() =>
        Assert.Equal("""<img src="u.png" alt="A cat" aria-label="A cat" aria-hidden="false" /><br>""",
            await Render(new ImageBlockModel { ImageUrl = "u.png", AltText = "A cat" }));

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task Image_WithoutAlt_IsAriaHidden(string? alt) =>
        Assert.Equal($"""<img src="u.png" alt="{alt}" aria-hidden="true" /><br>""",
            await Render(new ImageBlockModel { ImageUrl = "u.png", AltText = alt }));

    [Fact]
    public async Task Image_WithHeight_AddsHeightAttribute() =>
        Assert.Equal("""<img src="u.png" alt="A" aria-label="A" aria-hidden="false" height="120" /><br>""",
            await Render(new ImageBlockModel { ImageUrl = "u.png", AltText = "A", Height = 120 }));

    // ---- Column / group wrappers ----

    [Fact]
    public async Task Column_WrapsBlocksInClassedDiv()
    {
        var column = new EditorColumnModel
        {
            ColumnClass = "bb-layout-column--6",
            Blocks =
            [
                new EditorBlockModel { Block = new TitleBlockModel { Title = "T" } },
                new EditorBlockModel { Block = new QuoteBlockModel { Quote = "Q" } },
            ]
        };

        Assert.Equal("""<div class="bb-layout-column--6"><h1>T</h1><blockquote>Q</blockquote></div>""",
            await column.RenderAsync(_renderer));
    }

    [Fact]
    public async Task Column_SanitizesClass() =>
        Assert.Equal("""<div class="abc"></div>""",
            await new EditorColumnModel { ColumnClass = "a\"b<c>" }.RenderAsync(_renderer));

    [Fact]
    public async Task Group_WrapsColumns()
    {
        var group = new EditorGroupModel
        {
            CssClass = "bb-layout-grid",
            Columns =
            [
                new EditorColumnModel
                {
                    ColumnClass = "bb-layout-column--6",
                    Blocks = [new EditorBlockModel { Block = new RawTextBlockModel { Content = "x" } }]
                },
                new EditorColumnModel { ColumnClass = "bb-layout-column--6" },
            ]
        };

        Assert.Equal(
            """<div class="bb-layout-grid"><div class="bb-layout-column--6"><div>x</div></div><div class="bb-layout-column--6"></div></div>""",
            await group.RenderAsync(_renderer));
    }

    // ---- Unregistered models ----

    private sealed class UnregisteredModel : BaseBlockModel
    {
        public UnregisteredModel() : base(typeof(object)) { }
        public override string EditorName => "Unregistered";
    }

    [Fact]
    public async Task UnregisteredModel_Throws() =>
        await Assert.ThrowsAsync<InvalidOperationException>(() => Render(new UnregisteredModel()));

    // ---- Full pipeline from JSON ----

    [Fact]
    public async Task Pipeline_LoadedJson_ProducesExpectedHtml()
    {
        const string json =
"""{"Groups":[{"Columns":[{"ColumnClass":"bb-layout-column--6","Blocks":[{"Block":{"$modelType":"QuoteBlockModel","EditorName":"Quote","Quote":"abc123"}}]},{"ColumnClass":"bb-layout-column--6","Blocks":[]}],"GroupTypeName":"2 columns","CssClass":"bb-layout-grid"},{"Columns":[{"ColumnClass":"bb-layout-column--4","Blocks":[{"Block":{"$modelType":"RawTextBlockModel","EditorName":"Raw Text","Content":"Here's some <b>Raw</b> text"}}]},{"ColumnClass":"bb-layout-column--8","Blocks":[{"Block":{"$modelType":"ImageBlockModel","EditorName":"Image","ImageUrl":"https://example.com/i.jpg","AltText":""}}]}],"GroupTypeName":"1+2 columns","CssClass":"bb-layout-grid"}]}""";

        var model = new BlazorBlocksModel();
        model.Load(json);

        Assert.Equal(
            """<div class="bb-layout-grid"><div class="bb-layout-column--6"><blockquote>abc123</blockquote></div><div class="bb-layout-column--6"></div></div>""" +
            """<div class="bb-layout-grid"><div class="bb-layout-column--4"><div>Here's some <b>Raw</b> text</div></div><div class="bb-layout-column--8"><img src="https://example.com/i.jpg" alt="" aria-hidden="true" /><br></div></div>""",
            await model.GetHtmlAsync());
    }
}
