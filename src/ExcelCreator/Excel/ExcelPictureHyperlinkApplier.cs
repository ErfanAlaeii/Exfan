using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SpreadsheetPicture = DocumentFormat.OpenXml.Drawing.Spreadsheet.Picture;

namespace ExcelCreator.Excel;

public static class ExcelPictureHyperlinkApplier
{
    public static void Apply(string workbookPath, IReadOnlyList<ExcelImageReference> references)
    {
        if (references.Count == 0 || !File.Exists(workbookPath))
            return;

        var referencesByName = references.ToDictionary(
            reference => reference.PictureName,
            StringComparer.OrdinalIgnoreCase);

        using var document = SpreadsheetDocument.Open(workbookPath, true);
        var workbookPart = document.WorkbookPart;
        if (workbookPart?.Workbook?.Sheets == null)
            return;

        foreach (var sheetEntry in workbookPart.Workbook.Sheets.Elements<Sheet>())
        {
            if (sheetEntry.Id?.Value == null)
                continue;

            if (workbookPart.GetPartById(sheetEntry.Id.Value) is not WorksheetPart worksheetPart)
                continue;

            var drawingPart = worksheetPart.DrawingsPart;
            if (drawingPart?.WorksheetDrawing == null)
                continue;

            var changed = false;
            foreach (var anchor in drawingPart.WorksheetDrawing.Elements<OpenXmlCompositeElement>())
            {
                var picture = anchor switch
                {
                    TwoCellAnchor twoCell => twoCell.GetFirstChild<SpreadsheetPicture>(),
                    OneCellAnchor oneCell => oneCell.GetFirstChild<SpreadsheetPicture>(),
                    _ => null
                };

                if (picture?.NonVisualPictureProperties?.NonVisualDrawingProperties == null)
                    continue;

                var properties = picture.NonVisualPictureProperties.NonVisualDrawingProperties;
                var pictureName = properties.Name?.Value;
                if (string.IsNullOrWhiteSpace(pictureName))
                    continue;

                if (!referencesByName.TryGetValue(pictureName, out var reference))
                    continue;

                if (string.IsNullOrWhiteSpace(reference.HyperlinkTarget))
                    continue;

                var hyperlinkUri = reference.IsExternal
                    ? new Uri(reference.HyperlinkTarget, UriKind.Absolute)
                    : new Uri($"#{reference.HyperlinkTarget}", UriKind.Relative);

                var relationshipId = worksheetPart.AddHyperlinkRelationship(hyperlinkUri, true).Id;

                properties.HyperlinkOnClick = new HyperlinkOnClick
                {
                    Id = relationshipId,
                    Tooltip = reference.Tooltip
                };

                changed = true;
            }

            if (changed)
                drawingPart.WorksheetDrawing.Save();
        }

        document.Save();
    }
}
