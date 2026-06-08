using ExcelCreator.Core.Models;
using ExcelCreator.Application.Tables;
using FluentAssertions;

namespace ExcelCreator.Tests.Application.Tables;

public class TableSchemaResolverTests
{
    [Fact]
    public void ResolveSheet_UsesCustomColumnsWhenPresent()
    {
        var template = new TemplateDefinition
        {
            Id = "test",
            Workbook = new WorkbookSpec
            {
                Sheets =
                [
                    new SheetSpec
                    {
                        Columns = [new ColumnSpec { Header = "نام", Type = ColumnTypes.Text }]
                    }
                ]
            }
        };

        var table = new SavedTable
        {
            CustomColumns =
            [
                new ColumnSpec { Header = "نام", Type = ColumnTypes.Text },
                new ColumnSpec { Header = "یادداشت", Type = ColumnTypes.Text }
            ]
        };

        var sheet = TableSchemaResolver.ResolveSheet(template, table);

        sheet.Columns.Should().HaveCount(2);
        sheet.Columns[1].Header.Should().Be("یادداشت");
    }
}
