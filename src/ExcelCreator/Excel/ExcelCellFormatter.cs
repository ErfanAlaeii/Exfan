using ClosedXML.Excel;
using ExcelCreator.Infrastructure;
using ExcelCreator.Models;
using ExcelCreator.Services;
using System.Globalization;

namespace ExcelCreator.Excel;

public static class ExcelCellFormatter
{
    public static void ApplyTypedValue(IXLCell cell, ColumnSpec column, string value, DateCalendarKind calendar)
    {
        switch (column.Type.ToLowerInvariant())
        {
            case ColumnTypes.Number:
                if (double.TryParse(DateCalendarService.NormalizeDigits(value), NumberStyles.Any,
                        CultureInfo.InvariantCulture, out var number))
                    cell.Value = number;
                else
                    cell.Value = value;
                break;
            case ColumnTypes.Date:
                ApplyDateValue(cell, value, calendar);
                break;
            case ColumnTypes.Currency:
                if (decimal.TryParse(DateCalendarService.NormalizeDigits(value), NumberStyles.Any,
                        CultureInfo.InvariantCulture, out var money))
                {
                    cell.Value = money;
                    cell.Style.NumberFormat.Format = "#,##0";
                }
                else
                    cell.Value = value;
                break;
            default:
                cell.Value = value;
                break;
        }
    }

    public static void ApplyDateValue(IXLCell cell, string value, DateCalendarKind calendar)
    {
        if (!DateCalendarService.TryParseToGregorian(value, out var date))
        {
            cell.Value = value;
            return;
        }

        if (calendar == DateCalendarKind.Gregorian)
        {
            cell.Value = date;
            cell.Style.DateFormat.Format = "yyyy/mm/dd";
        }
        else
        {
            cell.Value = DateCalendarService.Format(date, DateCalendarKind.Jalali);
            cell.Style.NumberFormat.Format = "@";
        }
    }

    public static void ApplyDateColumnFormats(
        IXLWorksheet sheet,
        List<ColumnSpec> columns,
        int lastRow,
        DateCalendarKind calendar)
    {
        if (lastRow < 2)
            return;

        for (var col = 0; col < columns.Count; col++)
        {
            if (!ColumnTypes.IsDate(columns[col].Type))
                continue;

            var range = sheet.Range(2, col + 1, lastRow, col + 1);
            range.Style.NumberFormat.Format = DateCalendarService.GetExcelDateFormat(calendar);
        }
    }

    public static void ApplyHeaderRowStyle(IXLWorksheet sheet, List<ColumnSpec> columns)
    {
        for (var col = 0; col < columns.Count; col++)
        {
            var cell = sheet.Cell(1, col + 1);
            cell.Value = columns[col].Header;
            cell.Style.Font.Bold = true;
            cell.Style.Font.FontColor = XLColor.FromHtml(ExcelTheme.HeaderFontColor);
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml(ExcelTheme.HeaderBackgroundColor);
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            cell.Style.Alignment.ReadingOrder = XLAlignmentReadingOrderValues.RightToLeft;
        }
    }
}
