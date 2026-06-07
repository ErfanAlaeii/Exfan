using System.Data;
using ExcelCreator.Application.Common;
using ExcelCreator.Application.Images;
using ExcelCreator.Application.Tables;
using ExcelCreator.Core.Models;
using ExcelCreator.UI.Resources;

namespace ExcelCreator.UI.Presentation;

public static class TableRowGridPresenter
{
    public static DataTable BuildViewModel(SheetSpec sheet, IReadOnlyList<TableRow> rows, DateCalendarKind calendar)
    {
        var table = new DataTable();
        foreach (var column in sheet.Columns)
            table.Columns.Add(column.Header, typeof(string));
        table.Columns.Add(TableRowMapper.TimestampColumnHeader, typeof(string));

        var columnTypes = sheet.Columns.Select(c => c.Type).ToList();
        foreach (var row in rows)
        {
            var dataRow = table.NewRow();
            for (var i = 0; i < sheet.Columns.Count; i++)
            {
                var header = sheet.Columns[i].Header;
                var raw = i < row.Values.Count ? row.Values[i] : string.Empty;
                dataRow[header] = ColumnTypes.IsDate(columnTypes[i])
                    ? DateCalendarService.FormatDisplay(raw, calendar)
                    : ColumnTypes.IsImage(columnTypes[i])
                        ? ImageDisplayHelper.FormatGridValue(raw)
                        : raw;
            }

            dataRow[TableRowMapper.TimestampColumnHeader] =
                DateCalendarService.FormatDateTime(row.CreatedAt.ToLocalTime(), calendar);
            table.Rows.Add(dataRow);
        }

        return table;
    }

    public static double CalculateGridHeight(int rowCount) =>
        rowCount == 0 ? 0 : UiMetrics.GridHeaderHeight + rowCount * UiMetrics.GridRowHeight + 2;
}
