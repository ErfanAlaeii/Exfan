using ExcelCreator.Application.Print;
using ExcelCreator.Application.Tables;
using ExcelCreator.Core.Models;
using FluentAssertions;

namespace ExcelCreator.Tests.Application.Print;

public class TablePrintModelBuilderTests
{
    private static readonly SheetSpec Sheet = new()
    {
        Columns =
        [
            new ColumnSpec { Header = "نام", Type = ColumnTypes.Text },
            new ColumnSpec { Header = "تصویر", Type = ColumnTypes.Image }
        ]
    };

    [Fact]
    public void Build_IncludesHeadersAndFormattedRows()
    {
        var rows = new List<TableRow>
        {
            TableRow.FromValues(["علی", "C:\\files\\photo.jpg"], new DateTime(2026, 6, 7, 10, 0, 0, DateTimeKind.Local))
        };

        var model = TablePrintModelBuilder.Build(Sheet, rows, DateCalendarKind.Jalali, "بایگانی");

        model.Title.Should().Be("بایگانی");
        model.Headers.Should().Equal("نام", "تصویر", TableRowMapper.TimestampColumnHeader);
        model.Rows.Should().HaveCount(1);
        model.Rows[0][0].Should().Be("علی");
        model.Rows[0][1].Should().Be("photo.jpg");
        model.Rows[0][2].Should().NotBeNullOrWhiteSpace();
    }
}
