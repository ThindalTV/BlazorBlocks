using BlazorBlocks.Blocks.Markdown;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorBlocks.Blocks.Markdown.Tests;

public class MarkdownBlockTests : IAsyncLifetime
{
    private static readonly Lazy<bool> Registered = new(() =>
    {
        new ServiceCollection().AddBlazorBlocks().AddMarkdownBlock();
        return true;
    });

    private BlockHtmlRenderer _renderer = null!;

    public Task InitializeAsync()
    {
        _ = Registered.Value;
        _renderer = BlockHtmlRenderer.CreateStandalone();
        return Task.CompletedTask;
    }

    public async Task DisposeAsync() => await _renderer.DisposeAsync();

    private Task<string> Render(string markdown) => _renderer.RenderBlockAsync(new MarkdownBlockModel { Markdown = markdown });

    [Fact]
    public async Task RendersHeadingAndEmphasis() =>
        Assert.Equal("""<div class="bb-markdown"><h1>Title</h1>""" + "\n" + """<p>some <strong>bold</strong> and <em>italic</em></p></div>""",
            (await Render("# Title\n\nsome **bold** and *italic*")).Replace("\r\n", "\n"));

    [Fact]
    public async Task Empty_RendersEmptyContainer() =>
        Assert.Equal("""<div class="bb-markdown"></div>""", await Render(""));

    [Fact]
    public async Task RendersAdvancedExtensions_Tables()
    {
        var html = await Render("| a | b |\n|---|---|\n| 1 | 2 |");
        Assert.Contains("<table>", html);
        Assert.Contains("<td>1</td>", html);
    }

    [Fact]
    public async Task RawHtml_IsNotPassedThrough()
    {
        var html = await Render("hi <script>alert(1)</script> <img src=x onerror=alert(1)>");
        Assert.DoesNotContain("<script>", html);
        Assert.DoesNotContain("<img", html);
    }

    [Theory]
    [InlineData("[x](javascript:alert(1))")]
    [InlineData("[x](JaVaScRiPt:alert(1))")]
    [InlineData("[x](data:text/html;base64,AAAA)")]
    public async Task ScriptLinks_AreNeutralized(string markdown)
    {
        var html = await Render(markdown);
        Assert.DoesNotContain("javascript", html, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("data:", html);
        Assert.Contains("""href="#" """.TrimEnd(), html);
    }

    [Fact]
    public async Task NormalLinks_AreKept() =>
        Assert.Contains("""<a href="https://example.com">x</a>""", await Render("[x](https://example.com)"));

    [Fact]
    public void Model_SurvivesJsonRoundTrip()
    {
        _ = Registered.Value;
        var group = new BlazorBlocks.Internals.Models.EditorGroupModel
        {
            Columns = [new() { Blocks = [new() { Block = new MarkdownBlockModel { Markdown = "# Hi" } }] }]
        };
        var loaded = new BlazorBlocksModel();
        loaded.Load(new BlazorBlocksModel { Groups = [group] }.GetJson());

        var block = Assert.IsType<MarkdownBlockModel>(loaded.Groups[0].Columns[0].Blocks[0].Block);
        Assert.Equal("# Hi", block.Markdown);
    }
}
