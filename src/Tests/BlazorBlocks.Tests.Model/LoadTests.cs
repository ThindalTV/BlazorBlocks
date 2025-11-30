namespace BlazorBlocks.Tests.Model;

public class LoadTests
{
    [Fact]
    public void LoadSemiComplicatedJson()
    {
        // Arrange
        string t2 =
"""{"Groups":[{"Columns":[{"ColumnClass":"bb-layout-column--6","Blocks":[{"Block":{"$modelType":"QuoteBlockModel","EditorName":"Quote","Quote":"abc123"}}]},{"ColumnClass":"bb-layout-column--6","Blocks":[]}],"GroupTypeName":"2 columns","CssClass":"bb-layout-grid"},{"Columns":[{"ColumnClass":"bb-layout-column--4","Blocks":[{"Block":{"$modelType":"RawTextBlockModel","EditorName":"Raw Text","Content":"Here\u0027s some \u003Cb\u003ERaw\u003C/b\u003E text"}}]},{"ColumnClass":"bb-layout-column--8","Blocks":[{"Block":{"$modelType":"ImageBlockModel","EditorName":"Image","ImageUrl":"https://static3.bigstockphoto.com/3/8/3/large1500/383884268.jpg","AltText":""}}]}],"GroupTypeName":"1\u002B2 columns","CssClass":"bb-layout-grid"}]}""";
        BlazorBlocksModel model = new BlazorBlocksModel();

        // Act
        model.Load(t2);
        var html = model.GetHtml();
        
        // Assert
        Assert.NotNull(html);
    }
}
