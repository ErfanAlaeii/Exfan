using ExcelCreator.Application.Tables;
using ExcelCreator.Core.Models;
using FluentAssertions;

namespace ExcelCreator.Tests.Application.Tables;

public class TableRowSearchMatcherTests
{
    [Fact]
    public void FindMatchingRowIndices_FiltersByTextColumnContains()
    {
        var sheet = CreateSheet([("نام", ColumnTypes.Text)]);
        var rows = new List<TableRow>
        {
            Row("علی"),
            Row("رضا"),
            Row("علی‌رضا")
        };

        var criteria = new TableRowSearchCriteria { Target = TableRowSearchTarget.Column, ColumnIndex = 0, SearchValue = "علی" };
        var indices = TableRowSearchMatcher.FindMatchingRowIndices(rows, sheet, criteria, DateCalendarKind.Jalali);

        indices.Should().BeEquivalentTo([0, 2]);
    }

    [Fact]
    public void FindMatchingRowIndices_FiltersByExactDate()
    {
        var sheet = CreateSheet([("تاریخ", ColumnTypes.Date)]);
        var rows = new List<TableRow>
        {
            Row("1403/01/15"),
            Row("1403/02/01")
        };

        var criteria = new TableRowSearchCriteria { Target = TableRowSearchTarget.Column, ColumnIndex = 0, SearchValue = "2024-04-03" };
        var indices = TableRowSearchMatcher.FindMatchingRowIndices(rows, sheet, criteria, DateCalendarKind.Jalali);

        indices.Should().BeEquivalentTo([0]);
    }

    [Fact]
    public void FindMatchingRowIndices_FiltersByDropdownExactMatch()
    {
        var sheet = new SheetSpec
        {
            Columns =
            [
                new ColumnSpec
                {
                    Header = "وضعیت",
                    Type = ColumnTypes.Text,
                    DropdownValues = ["فعال", "غیرفعال"]
                }
            ]
        };
        var rows = new List<TableRow>
        {
            Row("فعال"),
            Row("غیرفعال")
        };

        var criteria = new TableRowSearchCriteria { Target = TableRowSearchTarget.Column, ColumnIndex = 0, SearchValue = "فعال" };
        var indices = TableRowSearchMatcher.FindMatchingRowIndices(rows, sheet, criteria, DateCalendarKind.Jalali);

        indices.Should().BeEquivalentTo([0]);
    }

    [Fact]
    public void FindMatchingRowIndices_FiltersByCreatedAtDate()
    {
        var sheet = CreateSheet([("نام", ColumnTypes.Text)]);
        var rows = new List<TableRow>
        {
            new() { Values = ["ali"], CreatedAt = new DateTime(2024, 6, 7, 16, 3, 0, DateTimeKind.Local) },
            new() { Values = ["reza"], CreatedAt = new DateTime(2024, 6, 8, 10, 0, 0, DateTimeKind.Local) }
        };

        var criteria = new TableRowSearchCriteria
        {
            Target = TableRowSearchTarget.CreatedAt,
            SearchValue = "2024-06-07"
        };
        var indices = TableRowSearchMatcher.FindMatchingRowIndices(rows, sheet, criteria, DateCalendarKind.Jalali);

        indices.Should().BeEquivalentTo([0]);
    }

    [Fact]
    public void Matches_NormalizesPersianDigits()
    {
        var column = new ColumnSpec { Header = "شماره", Type = ColumnTypes.Number };
        var row = Row("123");

        TableRowSearchMatcher.Matches(row, column, 0, "۱۲۳", DateCalendarKind.Jalali).Should().BeTrue();
    }

    private static SheetSpec CreateSheet(IReadOnlyList<(string Header, string Type)> columns) =>
        new()
        {
            Columns = columns
                .Select(pair => new ColumnSpec { Header = pair.Header, Type = pair.Type })
                .ToList()
        };

    private static TableRow Row(params string[] values) =>
        new() { Values = values.ToList() };
}
