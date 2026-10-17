using System.Text.Json;
using BlazorBlocks.Blocks.Wysiwyg;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorBlocks.Blocks.Wysiwyg.Tests;

public class WysiwygBlockTests : IAsyncLifetime
{
    private static readonly Lazy<bool> Registered = new(() =>
    {
        new ServiceCollection().AddBlazorBlocks().AddWysiwygBlock();
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

    private Task<string> Render(string content) => _renderer.RenderBlockAsync(new WysiwygBlockModel { Content = content });

    [Fact]
    public async Task RendersFormattedContent() =>
        Assert.Equal("""<div class="bb-wysiwyg"><p>Hello <strong>world</strong></p></div>""", await Render("<p>Hello <strong>world</strong></p>"));

    [Fact]
    public async Task Empty_RendersEmptyContainer() =>
        Assert.Equal("""<div class="bb-wysiwyg"></div>""", await Render(""));

    [Fact]
    public async Task KeepsInlineStyles()
    {
        var html = await Render("""<p style="text-align: center; color: red;">x</p>""");
        Assert.Contains("text-align", html);
    }

    [Fact]
    public async Task RemovesScriptsAndEventHandlers()
    {
        var html = await Render("""<p onclick="alert(1)">x</p><script>alert(2)</script><img src="a.png" onerror="alert(3)">""");
        Assert.DoesNotContain("script", html, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("onclick", html, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("onerror", html, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("<img", html);
    }

    [Fact]
    public async Task RemovesScriptLinks()
    {
        var html = await Render("""<a href="javascript:alert(1)">x</a>""");
        Assert.DoesNotContain("javascript:", html, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Serialization_StoresContentOnly()
    {
        var json = JsonSerializer.Serialize(new WysiwygBlockModel { Content = "<p>a</p>" });
        Assert.Contains("\"Content\"", json);
        Assert.DoesNotContain("\"Html\"", json);
    }
}
