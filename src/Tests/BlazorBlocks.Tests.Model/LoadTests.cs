using System.Text.Json.Nodes;
using BlazorBlocks.Blocks.ImageBlock;
using BlazorBlocks.Blocks.QuoteBlock;
using BlazorBlocks.Blocks.UnknownBlock;
using BlazorBlocks.Internals.Models;

namespace BlazorBlocks.Tests.Model;

public class LoadTests
{
    [Fact]
    public async Task LoadSemiComplicatedJson()
    {
        // Model is created BEFORE the blocks are registered (issue #19).
        BlazorBlocksModel model = new BlazorBlocksModel();
        TestRegistration.EnsureRegistered();

        // Arrange
        string t2 =
"""{"Groups":[{"Columns":[{"ColumnClass":"bb-layout-column--6","Blocks":[{"Block":{"$modelType":"QuoteBlockModel","EditorName":"Quote","Quote":"abc123"}}]},{"ColumnClass":"bb-layout-column--6","Blocks":[]}],"GroupTypeName":"2 columns","CssClass":"bb-layout-grid"},{"Columns":[{"ColumnClass":"bb-layout-column--4","Blocks":[{"Block":{"$modelType":"RawTextBlockModel","EditorName":"Raw Text","Content":"Here's some <b>Raw</b> text"}}]},{"ColumnClass":"bb-layout-column--8","Blocks":[{"Block":{"$modelType":"ImageBlockModel","EditorName":"Image","ImageUrl":"https://static3.bigstockphoto.com/3/8/3/large1500/383884268.jpg","AltText":""}}]}],"GroupTypeName":"1+2 columns","CssClass":"bb-layout-grid"}]}""";

        // Act
        model.Load(t2);
        var html = await model.GetHtmlAsync();

        // Assert
        Assert.NotNull(html);
    }

    [Fact]
    public void ImageHeight_SurvivesSaveAndLoad()
    {
        TestRegistration.EnsureRegistered();
        var source = new BlazorBlocksModel();
        var group = new EditorGroupModel();
        source.Groups = [group];
        var column = new EditorColumnModel();
        group.Columns = [column];
        column.Blocks = [new EditorBlockModel { Block = new ImageBlockModel { ImageUrl = "u.png", Height = 120 } }];

        var json = source.GetJson();
        var loaded = new BlazorBlocksModel();
        loaded.Load(json);

        var image = Assert.IsType<ImageBlockModel>(loaded.Groups[0].Columns[0].Blocks[0].Block);
        Assert.Equal(120, image.Height);
    }

    private const string WithRemovedBlock =
"""{"Groups":[{"Columns":[{"ColumnClass":"bb-layout-column--6","Blocks":[{"Block":{"$modelType":"QuoteBlockModel","Quote":"abc123"}},{"Block":{"$modelType":"GoneBlockModel","Text":"keep me","Nested":{"a":[1,2,3]}}}]}],"GroupTypeName":"1 column","CssClass":"bb-layout-grid"}]}""";

    [Fact]
    public void UnknownBlockType_LoadsAsPlaceholder()
    {
        TestRegistration.EnsureRegistered();
        var model = new BlazorBlocksModel();

        model.Load(WithRemovedBlock);

        var blocks = model.Groups[0].Columns[0].Blocks;
        Assert.IsType<QuoteBlockModel>(blocks[0].Block);
        var unknown = Assert.IsType<UnknownBlockModel>(blocks[1].Block);
        Assert.Equal("GoneBlockModel", unknown.OriginalType);
    }

    [Fact]
    public void UnknownBlockType_IsWrittenBackUnchanged()
    {
        TestRegistration.EnsureRegistered();
        var model = new BlazorBlocksModel();
        model.Load(WithRemovedBlock);

        var saved = JsonNode.Parse(model.GetJson())!;
        var original = JsonNode.Parse(WithRemovedBlock)!;

        Assert.True(JsonNode.DeepEquals(
            original["Groups"]![0]!["Columns"]![0]!["Blocks"]![1],
            saved["Groups"]![0]!["Columns"]![0]!["Blocks"]![1]));
    }

    [Fact]
    public async Task UnknownBlockType_IsSkippedInRenderedOutput()
    {
        TestRegistration.EnsureRegistered();
        var model = new BlazorBlocksModel();
        model.Load(WithRemovedBlock);

        var html = await model.GetHtmlAsync();

        Assert.Contains("abc123", html);
        Assert.DoesNotContain("keep me", html);
        Assert.DoesNotContain("Unknown", html);
    }
}
