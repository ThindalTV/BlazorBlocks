using BlazorBlocks.Blocks.ImageBlock;
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
"""{"Groups":[{"Columns":[{"ColumnClass":"bb-layout-column--6","Blocks":[{"Block":{"$modelType":"QuoteBlockModel","EditorName":"Quote","Quote":"abc123"}}]},{"ColumnClass":"bb-layout-column--6","Blocks":[]}],"GroupTypeName":"2 columns","CssClass":"bb-layout-grid"},{"Columns":[{"ColumnClass":"bb-layout-column--4","Blocks":[{"Block":{"$modelType":"RawTextBlockModel","EditorName":"Raw Text","Content":"Here\u0027s some \u003Cb\u003ERaw\u003C/b\u003E text"}}]},{"ColumnClass":"bb-layout-column--8","Blocks":[{"Block":{"$modelType":"ImageBlockModel","EditorName":"Image","ImageUrl":"https://static3.bigstockphoto.com/3/8/3/large1500/383884268.jpg","AltText":""}}]}],"GroupTypeName":"1\u002B2 columns","CssClass":"bb-layout-grid"}]}""";

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
}
