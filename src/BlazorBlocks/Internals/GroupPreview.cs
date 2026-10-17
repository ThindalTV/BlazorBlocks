using System.Text.RegularExpressions;
using BlazorBlocks.Services.Registrations;

namespace BlazorBlocks.Internals;

/// <summary>
/// Works out the miniature layout shown for a group in the "add group" dialog,
/// based on the column spans in the registered column classes (e.g. <c>bb-layout-column--6</c>).
/// </summary>
public static partial class GroupPreview
{
    private const int GridColumns = 12;

    [GeneratedRegex(@"(\d+)$")]
    private static partial Regex TrailingNumber();

    /// <summary>Gets the span (1-12) of a column from its class, or null when the class has no trailing number.</summary>
    public static int? GetSpan(string? columnClass)
    {
        var match = TrailingNumber().Match(columnClass?.Trim() ?? string.Empty);
        if (!match.Success || !int.TryParse(match.Value, out var span))
        {
            return null;
        }

        return Math.Clamp(span, 1, GridColumns);
    }

    /// <summary>
    /// Splits the group's columns into rows that wrap when the spans exceed the 12 column grid.
    /// If any column has no readable span, all columns get equal width in one row.
    /// </summary>
    public static IReadOnlyList<IReadOnlyList<int>> GetRows(GroupRegistration group)
    {
        var spans = group.Columns.Select(c => GetSpan(c.ColumnClass)).ToList();
        if (spans.Count == 0)
        {
            return [];
        }

        if (spans.Any(s => s is null))
        {
            return [Enumerable.Repeat(1, spans.Count).ToList()];
        }

        var rows = new List<IReadOnlyList<int>>();
        var current = new List<int>();
        var used = 0;
        foreach (var span in spans.Select(s => s!.Value))
        {
            if (used + span > GridColumns && current.Count > 0)
            {
                rows.Add(current);
                current = [];
                used = 0;
            }

            current.Add(span);
            used += span;
        }

        rows.Add(current);
        return rows;
    }
}
