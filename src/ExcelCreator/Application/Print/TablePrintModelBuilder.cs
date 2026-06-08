using System.Data;
using ExcelCreator.Application.Tables;
using ExcelCreator.Core.Models;
using ExcelCreator.UI.Presentation;

namespace ExcelCreator.Application.Print;

public static class TablePrintModelBuilder
{
    public static TablePrintModel Build(
        SheetSpec sheet,
        IReadOnlyList<TableRow> rows,
        DateCalendarKind calendar,
        string title)
    {
        var dataTable = TableRowGridPresenter.BuildViewModel(sheet, rows, calendar);
        var headers = dataTable.Columns
            .Cast<DataColumn>()
            .Select(column => column.ColumnName)
            .ToList();

        var printRows = dataTable.Rows
            .Cast<DataRow>()
            .Select(row => headers
                .Select(header => row[header]?.ToString() ?? string.Empty)
                .ToList() as IReadOnlyList<string>)
            .ToList();

        return new TablePrintModel
        {
            Title = title,
            Headers = headers,
            Rows = printRows
        };
    }
}
