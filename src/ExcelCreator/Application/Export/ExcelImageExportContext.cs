using System.IO;
using ClosedXML.Excel;
using ExcelCreator.Application.Images;
using ExcelCreator.Excel;
using ExcelCreator.Localization;

namespace ExcelCreator.Application.Export;

internal sealed class ExcelImageExportContext
{
    private int _imageSequence;

    public List<ExcelImageReference> References { get; } = [];

    public bool TryExportImageCell(
        IXLWorksheet dataSheet,
        int excelRow,
        int excelColumn,
        string filePath)
    {
        if (!File.Exists(filePath) || !MediaFileFormats.IsAllowed(filePath))
            return false;

        ApplyFileHyperlink(dataSheet.Cell(excelRow, excelColumn), filePath);

        if (!MediaFileFormats.IsRasterImage(filePath))
            return true;

        var sequence = ++_imageSequence;
        var pictureName = $"ExfanThumb_{sequence}";

        if (ExcelImageApplier.TryApplyThumbnail(
                dataSheet,
                excelRow,
                excelColumn,
                filePath,
                pictureName,
                out var reference) &&
            reference != null)
        {
            References.Add(reference);
        }

        return true;
    }

    private static void ApplyFileHyperlink(IXLCell cell, string filePath)
    {
        var fullPath = Path.GetFullPath(filePath);
        cell.SetHyperlink(new XLHyperlink(fullPath, PersianStrings.ExcelMediaClickTooltip));
        cell.Value = MediaFileFormats.IsPdf(filePath)
            ? PersianStrings.ExcelPdfOpenLink
            : PersianStrings.ExcelImageOpenLink;
        cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Bottom;
        cell.Style.Alignment.WrapText = true;
    }
}
