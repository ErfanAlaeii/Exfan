using ClosedXML.Excel;
using ExcelCreator.UI.Themes;
using ExcelCreator.Localization;
using ExcelCreator.Core.Models;

namespace ExcelCreator.Excel;

public static class SheetFeatureApplier
{
    public static void ApplyFeatures(IXLWorksheet sheet, SheetSpec sheetSpec, int columnCount, int lastRow)
    {
        if (columnCount == 0)
            return;

        var lastColLetter = sheet.Column(columnCount).ColumnLetter();
        var dataRange = sheet.Range($"A1:{lastColLetter}{lastRow}");
        var hasTableStyle = sheetSpec.Features.Any(f =>
            f.Equals(SheetFeatures.TableStyle, StringComparison.OrdinalIgnoreCase));

        foreach (var feature in sheetSpec.Features)
        {
            switch (feature.ToLowerInvariant())
            {
                case SheetFeatures.FreezeHeader:
                    sheet.SheetView.FreezeRows(1);
                    break;
                case SheetFeatures.TableStyle:
                    var table = dataRange.CreateTable();
                    table.Theme = XLTableTheme.TableStyleMedium2;
                    table.ShowAutoFilter = true;
                    table.ShowTotalsRow = sheetSpec.Columns.Any(c =>
                        c.Type.Equals(ColumnTypes.Number, StringComparison.OrdinalIgnoreCase) &&
                        string.IsNullOrEmpty(c.Formula));
                    break;
                case SheetFeatures.AutoFilter:
                    if (!hasTableStyle)
                        dataRange.SetAutoFilter();
                    break;
                case SheetFeatures.AlternateRows:
                    if (hasTableStyle || lastRow < 2)
                        break;
                    for (var r = 2; r <= lastRow; r += 2)
                        sheet.Range(r, 1, r, columnCount).Style.Fill.BackgroundColor =
                            XLColor.FromHtml(ExcelTheme.AlternateRowColor);
                    break;
            }
        }
    }

    public static void ApplyValidation(IXLWorksheet sheet, List<ColumnSpec> columns, int lastRow)
    {
        for (var col = 0; col < columns.Count; col++)
        {
            var dropdown = columns[col].DropdownValues;
            if (dropdown is not { Count: > 0 })
                continue;

            var range = sheet.Range(2, col + 1, lastRow, col + 1);
            var validation = range.CreateDataValidation();
            validation.List($"\"{string.Join(",", dropdown)}\"");
            validation.InCellDropdown = true;
            validation.ShowErrorMessage = true;
            validation.ErrorTitle = PersianStrings.ValidationErrorTitle;
            validation.ErrorMessage = string.Format(
                PersianStrings.ValidationErrorMessage,
                string.Join("، ", dropdown));
        }
    }
}
