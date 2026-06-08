using System.IO;
using System.IO.Compression;
using System.Xml.Linq;
using ClosedXML.Excel;
using ExcelCreator.Application.Export;
using ExcelCreator.Application.Templates;
using ExcelCreator.Core.Models;
using ExcelCreator.Application.Common;
using FluentAssertions;
using ExcelCreator.Tests.Helpers;

namespace ExcelCreator.Tests.Application.Export;

public class ExcelImageExportTests
{
    private static readonly byte[] MinimalPngBytes = Convert.FromBase64String(
        "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8z8BQDwAEhQGAhKmMIQAAAABJRU5ErkJggg==");

    private readonly ExcelWorkbookBuilder _builder = new();
    private readonly TemplateService _templates = new(new TestAppLogger(), TestPaths.TemplatesDirectory);

    [Fact]
    public void BuildAndSave_ArchiveTemplate_EmbedsClickableImageInCell()
    {
        var template = _templates.GetById("archive")!;
        var imagePath = Path.Combine(Path.GetTempPath(), $"Exfan_ExportImage_{Guid.NewGuid():N}.png");
        File.WriteAllBytes(imagePath, MinimalPngBytes);

        var outputPath = TestPaths.CreateTempFile();
        try
        {
            _builder.BuildAndSave(new GenerationRequest
            {
                Template = template,
                UserRows = [["1", "خدمات", imagePath]],
                DateCalendar = DateCalendarKind.Jalali
            }, outputPath);

            using var workbook = new XLWorkbook(outputPath);
            var sheet = workbook.Worksheet("بایگانی");
            sheet.Pictures.Should().NotBeEmpty();
            sheet.Cell(2, 3).GetHyperlink().Should().NotBeNull();
            sheet.Cell(2, 3).GetString().Should().Be("مشاهده تصویر");

            using var archive = ZipFile.OpenRead(outputPath);
            var sheetRels = archive.Entries.First(entry =>
                entry.FullName.Equals("xl/worksheets/_rels/sheet1.xml.rels", StringComparison.OrdinalIgnoreCase));
            using var relStream = sheetRels.Open();
            var relXml = XDocument.Load(relStream);
            var hyperlinkTarget = relXml.Descendants()
                .First(element => element.Name.LocalName == "Relationship" &&
                                  (string?)element.Attribute("Type") == "http://schemas.openxmlformats.org/officeDocument/2006/relationships/hyperlink")
                .Attribute("Target")?.Value;

            hyperlinkTarget.Should().Contain("Exfan_ExportImage_");
            hyperlinkTarget.Should().EndWith(".png");

            var drawingEntry = archive.Entries.First(entry =>
                entry.FullName.StartsWith("xl/drawings/drawing", StringComparison.OrdinalIgnoreCase));
            using var drawingStream = drawingEntry.Open();
            var drawingXml = XDocument.Load(drawingStream);
            drawingXml.Descendants().Any(element => element.Name.LocalName == "hlinkClick").Should().BeTrue();
        }
        finally
        {
            if (File.Exists(imagePath))
                File.Delete(imagePath);
            if (File.Exists(outputPath))
                File.Delete(outputPath);
        }
    }

    [Fact]
    public void BuildAndSave_ArchiveTemplate_ExportsPdfWithHyperlinkOnly()
    {
        var template = _templates.GetById("archive")!;
        var pdfPath = Path.Combine(Path.GetTempPath(), $"Exfan_ExportPdf_{Guid.NewGuid():N}.pdf");
        File.WriteAllText(pdfPath, "%PDF-1.0\n%%EOF");

        var outputPath = TestPaths.CreateTempFile();
        try
        {
            _builder.BuildAndSave(new GenerationRequest
            {
                Template = template,
                UserRows = [["2", "سند", pdfPath]],
                DateCalendar = DateCalendarKind.Jalali
            }, outputPath);

            using var workbook = new XLWorkbook(outputPath);
            var sheet = workbook.Worksheet("بایگانی");
            sheet.Pictures.Should().BeEmpty();
            sheet.Cell(2, 3).GetHyperlink().Should().NotBeNull();
            sheet.Cell(2, 3).GetString().Should().Be("مشاهده PDF");
        }
        finally
        {
            if (File.Exists(pdfPath))
                File.Delete(pdfPath);
            if (File.Exists(outputPath))
                File.Delete(outputPath);
        }
    }
}
