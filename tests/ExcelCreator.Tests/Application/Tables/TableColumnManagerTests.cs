using ExcelCreator.Application.Tables;
using ExcelCreator.Core.Models;
using FluentAssertions;

namespace ExcelCreator.Tests.Application.Tables;

public class TableColumnManagerTests
{
    private static SheetSpec CreateSheet(params string[] headers) =>
        new()
        {
            Columns = headers
                .Select(header => new ColumnSpec { Header = header, Type = ColumnTypes.Text })
                .ToList()
        };

    [Fact]
    public void AddColumn_AppendsTextColumn()
    {
        var sheet = CreateSheet("نام");
        var result = TableColumnManager.AddColumn(sheet, "توضیحات");

        result.IsValid.Should().BeTrue();
        sheet.Columns.Should().HaveCount(2);
        sheet.Columns[1].Header.Should().Be("توضیحات");
    }

    [Fact]
    public void AddColumn_RejectsDuplicateName()
    {
        var sheet = CreateSheet("نام");
        TableColumnManager.AddColumn(sheet, "توضیحات").IsValid.Should().BeTrue();

        TableColumnManager.AddColumn(sheet, "توضیحات").IsValid.Should().BeFalse();
    }

    [Fact]
    public void RemoveColumn_RemovesValuesFromRows()
    {
        var sheet = CreateSheet("نام", "ساعت");
        var rows = new List<TableRow>
        {
            TableRow.CreateNew(["علی", "10"]),
            TableRow.CreateNew(["رضا", "12"])
        };

        var result = TableColumnManager.RemoveColumn(sheet, 1, rows);

        result.IsValid.Should().BeTrue();
        sheet.Columns.Should().HaveCount(1);
        rows[0].Values.Should().Equal("علی");
        rows[1].Values.Should().Equal("رضا");
    }

    [Fact]
    public void RemoveColumn_RejectsLastColumn()
    {
        var sheet = CreateSheet("نام");
        var rows = new List<TableRow> { TableRow.CreateNew(["علی"]) };

        TableColumnManager.RemoveColumn(sheet, 0, rows).IsValid.Should().BeFalse();
    }

    [Fact]
    public void RenameColumn_UpdatesHeader()
    {
        var sheet = CreateSheet("نام");
        var result = TableColumnManager.RenameColumn(sheet, 0, "نام پرسنل");

        result.IsValid.Should().BeTrue();
        sheet.Columns[0].Header.Should().Be("نام پرسنل");
    }
}
