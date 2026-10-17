using BlazorBlocks.Internals;
using BlazorBlocks.Services.Registrations;

namespace BlazorBlocks.Tests.Model;

public class GroupPreviewTests
{
    private static GroupRegistration Group(params string[] columnClasses) =>
        new("test", "bb-layout-grid", columnClasses.Select(c => new ColumnRegistration(c)).ToList());

    [Theory]
    [InlineData("bb-layout-column--6", 6)]
    [InlineData("bb-layout-column--12", 12)]
    [InlineData("  bb-layout-column--4 ", 4)]
    [InlineData("bb-layout-column--99", 12)]
    [InlineData("bb-layout-column--0", 1)]
    [InlineData("custom-column", null)]
    [InlineData("", null)]
    [InlineData(null, null)]
    public void GetSpan_ReadsTrailingNumber(string? columnClass, int? expected) =>
        Assert.Equal(expected, GroupPreview.GetSpan(columnClass));

    [Fact]
    public void GetRows_SingleRow()
    {
        var rows = GroupPreview.GetRows(Group("bb-layout-column--4", "bb-layout-column--8"));
        Assert.Equal([[4, 8]], rows);
    }

    [Fact]
    public void GetRows_WrapsWhenSpansExceedTwelve()
    {
        var rows = GroupPreview.GetRows(Group("bb-layout-column--4", "bb-layout-column--8", "bb-layout-column--12"));
        Assert.Equal([[4, 8], [12]], rows);
    }

    [Fact]
    public void GetRows_UnreadableSpan_FallsBackToEqualWidths()
    {
        var rows = GroupPreview.GetRows(Group("bb-layout-column--4", "custom"));
        Assert.Equal([[1, 1]], rows);
    }

    [Fact]
    public void GetRows_NoColumns_IsEmpty() =>
        Assert.Empty(GroupPreview.GetRows(Group()));
}
