using System.IO;
using ClosedXML.Excel;
using ClosedXML.Excel.Drawings;
using ExcelCreator.Application.Images;
using ExcelCreator.Localization;

namespace ExcelCreator.Excel;

public static class ExcelImageApplier
{
    private const int CellPaddingPx = 6;
    private const int ThumbnailMaxWidthPx = 112;
    private const int ThumbnailMaxHeightPx = 72;
    private const int LinkAreaHeightPx = 22;
    private const double ThumbnailRowHeightPoints = 78;
    private const double ThumbnailColumnWidth = 16;

    public static bool TryApplyThumbnail(
        IXLWorksheet sheet,
        int row,
        int column,
        string imagePath,
        string pictureName,
        out ExcelImageReference? reference)
    {
        reference = null;
        if (!File.Exists(imagePath) || !MediaFileFormats.IsRasterImage(imagePath))
            return false;

        try
        {
            var cell = sheet.Cell(row, column);
            sheet.Row(row).Height = Math.Max(sheet.Row(row).Height, ThumbnailRowHeightPoints);
            sheet.Column(column).Width = Math.Max(sheet.Column(column).Width, ThumbnailColumnWidth);

            var picture = sheet.AddPicture(imagePath);
            picture.Name = pictureName;

            var scale = CalculateScale(
                picture.OriginalWidth,
                picture.OriginalHeight,
                ThumbnailMaxWidthPx,
                ThumbnailMaxHeightPx);

            var widthPx = Math.Max(1, (int)Math.Round(picture.OriginalWidth * scale));
            var heightPx = Math.Max(1, (int)Math.Round(picture.OriginalHeight * scale));

            picture
                .MoveTo(
                    cell,
                    CellPaddingPx,
                    CellPaddingPx,
                    cell,
                    CellPaddingPx + widthPx,
                    CellPaddingPx + heightPx + LinkAreaHeightPx)
                .WithPlacement(XLPicturePlacement.MoveAndSize);

            var fullPath = Path.GetFullPath(imagePath);
            reference = new ExcelImageReference(
                pictureName,
                HyperlinkTarget: new Uri(fullPath).AbsoluteUri,
                IsExternal: true,
                Tooltip: PersianStrings.ExcelMediaClickTooltip);

            return true;
        }
        catch
        {
            return false;
        }
    }

    private static double CalculateScale(int originalWidth, int originalHeight, int maxWidthPx, int maxHeightPx)
    {
        if (originalWidth <= 0 || originalHeight <= 0)
            return 1;

        var widthScale = maxWidthPx / (double)originalWidth;
        var heightScale = maxHeightPx / (double)originalHeight;
        return Math.Min(1.0, Math.Min(widthScale, heightScale));
    }
}
