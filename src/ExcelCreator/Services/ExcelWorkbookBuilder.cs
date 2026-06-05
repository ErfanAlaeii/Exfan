using ClosedXML.Excel;
using ExcelCreator.Abstractions;
using ExcelCreator.Excel;
using ExcelCreator.Infrastructure;
using ExcelCreator.Models;

namespace ExcelCreator.Services;

public sealed class ExcelWorkbookBuilder : IExcelExporter
{
    public void Export(GenerationRequest request, string outputPath) => BuildAndSave(request, outputPath);

    public void BuildAndSave(GenerationRequest request, string outputPath)
    {
        using var workbook = new XLWorkbook();
        var spec = request.Template.Workbook;

        foreach (var sheetSpec in spec.Sheets)
            AddDataSheet(workbook, sheetSpec, request);

        if (workbook.Worksheets.Count > 0)
            workbook.Worksheets.First().SetTabActive();

        workbook.SaveAs(outputPath);
    }

    private static void AddDataSheet(XLWorkbook workbook, SheetSpec sheetSpec, GenerationRequest request)
    {
        var sheet = workbook.Worksheets.Add(SanitizeSheetName(sheetSpec.Name));
        var columns = sheetSpec.Columns;
        if (columns.Count == 0)
            return;

        for (var col = 0; col < columns.Count; col++)
        {
            var width = columns[col].Width is double w and > 0 ? w : UiMetrics.DefaultExcelColumnWidth;
            sheet.Column(col + 1).Width = width;
        }

        var dataRows = request.UserRows;
        var lastDataRow = 1;

        for (var rowIndex = 0; rowIndex < dataRows.Count; rowIndex++)
        {
            var excelRow = rowIndex + 2;
            lastDataRow = excelRow;
            var rowValues = dataRows[rowIndex];

            for (var col = 0; col < columns.Count; col++)
            {
                var cell = sheet.Cell(excelRow, col + 1);
                var value = col < rowValues.Count ? rowValues[col] : string.Empty;

                if (!string.IsNullOrEmpty(columns[col].Formula))
                {
                    var formula = columns[col].Formula!.Replace("{row}", excelRow.ToString());
                    cell.FormulaA1 = formula;
                }
                else if (!string.IsNullOrWhiteSpace(value))
                {
                    ExcelCellFormatter.ApplyTypedValue(cell, columns[col], value, request.DateCalendar);
                }
            }
        }

        sheet.RightToLeft = true;
        ExcelCellFormatter.ApplyDateColumnFormats(sheet, columns, lastDataRow, request.DateCalendar);
        SheetFeatureApplier.ApplyFeatures(sheet, sheetSpec, columns.Count, Math.Max(lastDataRow, 1));
        ExcelCellFormatter.ApplyHeaderRowStyle(sheet, columns);
        SheetFeatureApplier.ApplyValidation(sheet, columns, Math.Max(lastDataRow, 1));
    }

    private static string SanitizeSheetName(string name)
    {
        var invalid = new[] { '\\', '/', '*', '?', ':', '[', ']' };
        var sanitized = new string(name.Select(c => invalid.Contains(c) ? '_' : c).ToArray());
        return sanitized.Length > 31 ? sanitized[..31] : sanitized;
    }
}
