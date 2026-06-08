using System.IO;
using ExcelCreator.Application.Common;
using ExcelCreator.Application.Images;
using ExcelCreator.Core.Models;

namespace ExcelCreator.Application.Tables;

public static class TableRowSearchMatcher
{
    public static IReadOnlyList<int> FindMatchingRowIndices(
        IReadOnlyList<TableRow> rows,
        SheetSpec sheet,
        TableRowSearchCriteria criteria,
        DateCalendarKind calendar)
    {
        if (criteria.Target == TableRowSearchTarget.CreatedAt)
            return FindCreatedAtMatches(rows, criteria.SearchValue);

        if (criteria.ColumnIndex < 0 || criteria.ColumnIndex >= sheet.Columns.Count)
            return [];

        var column = sheet.Columns[criteria.ColumnIndex];
        var indices = new List<int>();
        for (var index = 0; index < rows.Count; index++)
        {
            if (Matches(rows[index], column, criteria.ColumnIndex, criteria.SearchValue, calendar))
                indices.Add(index);
        }

        return indices;
    }

    public static bool Matches(
        TableRow row,
        ColumnSpec column,
        int columnIndex,
        string searchValue,
        DateCalendarKind calendar)
    {
        if (string.IsNullOrWhiteSpace(searchValue))
            return false;

        var raw = columnIndex < row.Values.Count ? row.Values[columnIndex] ?? string.Empty : string.Empty;

        if (ColumnTypes.IsDate(column.Type))
            return MatchesDate(raw, searchValue);

        if (column.DropdownValues is { Count: > 0 })
        {
            return string.Equals(
                Normalize(raw),
                Normalize(searchValue),
                StringComparison.OrdinalIgnoreCase);
        }

        if (ColumnTypes.IsImage(column.Type))
            return MatchesImage(raw, searchValue);

        return Normalize(raw).Contains(Normalize(searchValue), StringComparison.OrdinalIgnoreCase);
    }

    private static IReadOnlyList<int> FindCreatedAtMatches(IReadOnlyList<TableRow> rows, string searchValue)
    {
        if (!DateCalendarService.TryParseToGregorian(searchValue, out var searchDate))
            return [];

        var indices = new List<int>();
        for (var index = 0; index < rows.Count; index++)
        {
            if (rows[index].CreatedAt.ToLocalTime().Date == searchDate.Date)
                indices.Add(index);
        }

        return indices;
    }

    private static bool MatchesDate(string rawValue, string searchValue)
    {
        if (!DateCalendarService.TryParseToGregorian(searchValue, out var searchDate))
            return false;

        if (!DateCalendarService.TryParseToGregorian(rawValue, out var rowDate))
            return false;

        return searchDate.Date == rowDate.Date;
    }

    private static bool MatchesImage(string storedPath, string searchValue)
    {
        var normalizedSearch = Normalize(searchValue);
        var display = Normalize(ImageDisplayHelper.FormatGridValue(storedPath));
        if (display.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase))
            return true;

        var fileName = Normalize(Path.GetFileName(storedPath));
        return fileName.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase);
    }

    private static string Normalize(string value) =>
        DateCalendarService.NormalizeDigits(value.Trim());
}
