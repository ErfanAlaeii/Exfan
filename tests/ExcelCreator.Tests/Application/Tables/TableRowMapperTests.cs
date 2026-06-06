using ExcelCreator.Core.Models;
using ExcelCreator.Application.Tables;
using ExcelCreator.Application.Common;
using FluentAssertions;

namespace ExcelCreator.Tests.Application.Tables;

public class TableRowMapperTests
{
    private static readonly SheetSpec Sheet = new()
    {
        Columns =
        [
            new ColumnSpec { Header = "نام", Type = ColumnTypes.Text },
            new ColumnSpec { Header = "مبلغ", Type = ColumnTypes.Currency }
        ]
    };

    [Fact]
    public void WithTimestampColumn_AppendsTimestampHeader()
    {
        var template = new TemplateDefinition
        {
            Id = "test",
            Workbook = new WorkbookSpec { Sheets = [Sheet] }
        };

        var exportTemplate = TableRowMapper.WithTimestampColumn(template);
        var columns = exportTemplate.RequirePrimarySheet().Columns;

        columns.Should().HaveCount(3);
        columns[^1].Header.Should().Be(TableRowMapper.TimestampColumnHeader);
    }

    [Fact]
    public void ToExportValues_AppendsFormattedTimestamp()
    {
        var createdAt = new DateTime(2026, 6, 5, 10, 30, 0, DateTimeKind.Utc);
        var rows = new List<TableRow>
        {
            TableRow.FromValues(["علی", "1000"], createdAt)
        };

        var exportValues = TableRowMapper.ToExportValues(rows, DateCalendarKind.Gregorian);

        exportValues.Should().HaveCount(1);
        exportValues[0].Should().HaveCount(3);
        exportValues[0][0].Should().Be("علی");
        exportValues[0][1].Should().Be("1000");
        exportValues[0][2].Should().MatchRegex(@"^\d{4}/\d{2}/\d{2} \d{2}:\d{2}$");
    }
}
