using BlazorBlocks.Blocks.ImageBlock;
using BlazorBlocks.Blocks.TitleBlock;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorBlocks.Tests.Model;

/// <summary>
/// A host that saves on <c>OnModelUpdated</c> loses any edit that does not raise it,
/// so every kind of change is checked here (issue #33).
/// </summary>
public class ModelUpdatedTests : TestContext
{
    private const string Quote = """{"$modelType":"QuoteBlockModel","Quote":"q"}""";
    private const string Title = """{"$modelType":"TitleBlockModel","Title":"t","TitleSize":0}""";
    private const string RawText = """{"$modelType":"RawTextBlockModel","Content":"c"}""";
    private const string Image = """{"$modelType":"ImageBlockModel","ImageUrl":"u.png","AltText":"a"}""";
    private const string Svg = """{"$modelType":"SvgBlockModel","Markup":"<svg/>","AltText":"a"}""";

    private int _updates;

    public ModelUpdatedTests()
    {
        TestRegistration.EnsureRegistered();
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddBlazorBlocks(includeDefaultGroups: false, includeDefaultBlocks: false);
    }

    private static string Column(params string[] blocks) =>
        $$"""{"ColumnClass":"bb-layout-column--6","Blocks":[{{string.Join(",", blocks.Select(b => $$"""{"Block":{{b}}}"""))}}]}""";

    private static string Group(params string[] columns) =>
        $$"""{"Columns":[{{string.Join(",", columns)}}],"GroupTypeName":"group","CssClass":"bb-layout-grid"}""";

    private static BlazorBlocksModel ModelOf(params string[] groups)
    {
        var model = new BlazorBlocksModel();
        model.Load($$"""{"Groups":[{{string.Join(",", groups)}}]}""");
        return model;
    }

    private IRenderedComponent<BlazorBlocksEditor> RenderEditor(BlazorBlocksModel model) =>
        RenderComponent<BlazorBlocksEditor>(p => p
            .Add(c => c.Model, model)
            .Add(c => c.OnModelUpdated, EventCallback.Factory.Create(this, () => _updates++)));

    private static void ConfirmDelete(IRenderedComponent<BlazorBlocksEditor> editor) =>
        editor.Find(".bb-button--primary").Click();

    // ---- Structure ----

    [Fact]
    public void AddingAGroup_RaisesOnModelUpdated()
    {
        var model = ModelOf();
        var editor = RenderEditor(model);

        editor.Find(".bb-editor__add-row-button--group").Click();
        editor.FindAll("button.bb-dialog-tile").First(b => b.TextContent.Contains("1 column")).Click();

        Assert.Single(model.Groups);
        Assert.Equal(1, _updates);
    }

    [Fact]
    public void DeletingAGroup_RaisesOnModelUpdated()
    {
        var model = ModelOf(Group(Column()));
        var editor = RenderEditor(model);

        editor.Find(".bb-draggable__sidebar > span:last-child").Click();
        ConfirmDelete(editor);

        Assert.Empty(model.Groups);
        Assert.Equal(1, _updates);
    }

    [Fact]
    public void AddingABlock_RaisesOnModelUpdated()
    {
        var model = ModelOf(Group(Column()));
        var editor = RenderEditor(model);

        editor.Find(".bb-editor__add-row-button--block").Click();
        editor.FindAll("button.bb-dialog-tile").First(b => b.TextContent.Contains("Quote")).Click();

        Assert.Single(model.Groups[0].Columns[0].Blocks);
        Assert.Equal(1, _updates);
    }

    [Fact]
    public void DeletingABlock_RaisesOnModelUpdated()
    {
        var model = ModelOf(Group(Column(Quote)));
        var editor = RenderEditor(model);

        editor.Find(".bb-draggable__header__row > span:last-child").Click();
        ConfirmDelete(editor);

        Assert.Empty(model.Groups[0].Columns[0].Blocks);
        Assert.Equal(1, _updates);
    }

    // ---- Drag and drop ----

    [Fact]
    public void MovingAGroup_RaisesOnModelUpdated()
    {
        var model = ModelOf(Group(Column()), Group(Column()));
        var first = model.Groups[0];
        var editor = RenderEditor(model);

        editor.Find(".bb-draggable--group .bb-draggable__handle").TriggerEvent("ondragstart", new DragEventArgs());
        editor.FindAll(".bb-editor > .bb-editor__add-row").Last().TriggerEvent("ondrop", new DragEventArgs());

        Assert.Same(first, model.Groups[1]);
        Assert.Equal(1, _updates);
    }

    [Fact]
    public void MovingABlockWithinAColumn_RaisesOnModelUpdated()
    {
        var model = ModelOf(Group(Column(Quote, RawText)));
        var first = model.Groups[0].Columns[0].Blocks[0];
        var editor = RenderEditor(model);

        editor.Find(".bb-draggable--block .bb-draggable__handle").TriggerEvent("ondragstart", new DragEventArgs());
        editor.FindAll(".bb-editor__column > .bb-editor__add-row").Last().TriggerEvent("ondrop", new DragEventArgs());

        Assert.Same(first, model.Groups[0].Columns[0].Blocks[1]);
        Assert.Equal(1, _updates);
    }

    [Fact]
    public void MovingABlockToAnotherColumn_RaisesOnModelUpdated()
    {
        var model = ModelOf(Group(Column(Quote), Column()));
        var block = model.Groups[0].Columns[0].Blocks[0];
        var editor = RenderEditor(model);

        editor.Find(".bb-draggable--block .bb-draggable__handle").TriggerEvent("ondragstart", new DragEventArgs());
        editor.FindAll(".bb-editor__column")[1].QuerySelector(".bb-editor__add-row")!.TriggerEvent("ondrop", new DragEventArgs());

        Assert.Empty(model.Groups[0].Columns[0].Blocks);
        Assert.Same(block, model.Groups[0].Columns[1].Blocks[0]);
        Assert.Equal(1, _updates);
    }

    // ---- Block fields ----

    [Theory]
    [InlineData(Quote, "input[type=text]", "changed")]
    [InlineData(Title, "#titleText", "changed")]
    [InlineData(RawText, "textarea", "changed")]
    [InlineData(Image, "#imageSrc", "changed.png")]
    [InlineData(Image, "#imageAlt", "changed")]
    [InlineData(Image, "#imageHeight", "50")]
    [InlineData(Svg, "#svgAlt", "changed")]
    public void ChangingABlockField_RaisesOnModelUpdated(string block, string selector, string value)
    {
        var editor = RenderEditor(ModelOf(Group(Column(block))));

        editor.Find(selector).Change(value);

        Assert.Equal(1, _updates);
    }

    [Fact]
    public void ChangingTheSvgMarkup_RaisesOnModelUpdated()
    {
        var editor = RenderEditor(ModelOf(Group(Column(Svg))));

        editor.Find("#svgMarkup").Input("<svg><g/></svg>");

        Assert.Equal(1, _updates);
    }

    [Fact]
    public void ChangingTheTitleSize_RaisesOnModelUpdated()
    {
        var editor = RenderEditor(ModelOf(Group(Column(Title))));

        editor.Find("#titleSize").Change(nameof(TitleSizeEnum.Small));

        Assert.Equal(1, _updates);
    }

    [Fact]
    public void ChangingTheImageSource_RaisesOnModelUpdated()
    {
        var editor = RenderEditor(ModelOf(Group(Column(Image))));

        editor.Find("#imageSource").Change(nameof(ImageSourceMode.Embedded));

        Assert.Equal(1, _updates);
    }
}
