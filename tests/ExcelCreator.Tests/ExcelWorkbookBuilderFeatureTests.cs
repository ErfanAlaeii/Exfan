using ClosedXML.Excel;
using ExcelCreator.Models;
using ExcelCreator.Services;
using FluentAssertions;
using ExcelCreator.Tests.Helpers;

namespace ExcelCreator.Tests;

public class ExcelWorkbookBuilderFeatureTests
{
    [Fact]
    public void BuildAndSave_PersonnelReport_HasPlainHeaderStyle()
    {
        var template = new TemplateService(new TestAppLogger(), TestPaths.TemplatesDirectory).GetById("personnel-activity-report")!;
        var path = TestPaths.CreateTempFile();
        try
        {
            new ExcelWorkbookBuilder().BuildAndSave(new GenerationRequest
            {
                Template = template,
                DateCalendar = DateCalendarKind.Jalali
            }, path);

            using var wb = new XLWorkbook(path);
            var sheet = wb.Worksheet("فعالیت پرسنل");
            var header = sheet.Cell(1, 1);
            header.Style.Font.Bold.Should().BeTrue();
            header.Style.Font.FontColor.Should().Be(XLColor.Black);
            header.Style.Fill.BackgroundColor.Should().Be(XLColor.White);
            sheet.AutoFilter.IsEnabled.Should().BeFalse();
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }
}
