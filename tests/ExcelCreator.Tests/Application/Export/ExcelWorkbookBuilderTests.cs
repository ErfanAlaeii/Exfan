using ClosedXML.Excel;
using ExcelCreator.Core.Models;
using ExcelCreator.Application.Export;
using ExcelCreator.Application.Common;
using ExcelCreator.Application.Templates;
using FluentAssertions;
using ExcelCreator.Tests.Helpers;

namespace ExcelCreator.Tests.Application.Export;

public class ExcelWorkbookBuilderTests
{
    private readonly ExcelWorkbookBuilder _builder = new();
    private readonly TemplateService _templates = new(new TestAppLogger(), TestPaths.FixturesDirectory);

    [Fact]
    public void BuildAndSave_CreatesValidXlsxFile()
    {
        var template = _templates.GetById("test-minimal")!;
        var path = TestPaths.CreateTempFile();
        try
        {
            _builder.BuildAndSave(new GenerationRequest
            {
                Template = template,
                DateCalendar = DateCalendarKind.Jalali
            }, path);

            File.Exists(path).Should().BeTrue();
            new FileInfo(path).Length.Should().BeGreaterThan(1024);

            using var wb = new XLWorkbook(path);
            wb.Worksheets.Should().HaveCount(1);
            wb.Worksheets.First().Name.Should().Be("داده");
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    [Fact]
    public void BuildAndSave_JalaliDate_WritesTextInDataSheet()
    {
        var template = _templates.GetById("test-minimal")!;
        var path = TestPaths.CreateTempFile();
        try
        {
            _builder.BuildAndSave(new GenerationRequest
            {
                Template = template,
                UserRows = [["نمونه", "2026-06-02", "1000"]],
                DateCalendar = DateCalendarKind.Jalali
            }, path);

            using var wb = new XLWorkbook(path);
            var sheet = wb.Worksheet("داده");
            var dateCell = sheet.Cell(2, 2);

            dateCell.GetString().Should().Be(DateCalendarService.Format(
                new DateTime(2026, 6, 2), DateCalendarKind.Jalali));
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    [Fact]
    public void BuildAndSave_GregorianDate_WritesDateTimeValue()
    {
        var template = _templates.GetById("test-minimal")!;
        var path = TestPaths.CreateTempFile();
        try
        {
            _builder.BuildAndSave(new GenerationRequest
            {
                Template = template,
                UserRows = [["نمونه", "2026-06-02", "1000"]],
                DateCalendar = DateCalendarKind.Gregorian
            }, path);

            using var wb = new XLWorkbook(path);
            var sheet = wb.Worksheet("داده");
            var dateCell = sheet.Cell(2, 2);

            dateCell.DataType.Should().Be(XLDataType.DateTime);
            dateCell.GetDateTime().Date.Should().Be(new DateTime(2026, 6, 2));
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    [Fact]
    public void BuildAndSave_WithUserRows_WritesDataToFirstSheet()
    {
        var templates = new TemplateService(new TestAppLogger(), TestPaths.TemplatesDirectory);
        var template = templates.GetById("personnel-activity-report")!;
        var path = TestPaths.CreateTempFile();
        try
        {
            _builder.BuildAndSave(new GenerationRequest
            {
                Template = template,
                UserRows = [["علی رضایی", "08:00", "17:00", "پاسخ به ایمیل‌ها"]],
                DateCalendar = DateCalendarKind.Jalali
            }, path);

            using var wb = new XLWorkbook(path);
            wb.Worksheets.First().Name.Should().Be("فعالیت پرسنل");
            var sheet = wb.Worksheet("فعالیت پرسنل");
            sheet.Cell(1, 1).GetString().Should().Be("نام");
            sheet.Cell(1, 4).GetString().Should().Be("شرح کار");
            sheet.Cell(2, 1).GetString().Should().Be("علی رضایی");
            sheet.Cell(2, 2).GetString().Should().Be("08:00");
            sheet.Cell(2, 3).GetString().Should().Be("17:00");
            sheet.Cell(2, 4).GetString().Should().Be("پاسخ به ایمیل‌ها");
            wb.Worksheets.Should().HaveCount(1);
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    [Fact]
    public void BuildAndSave_PersonnelActivityReport_HasPersianColumns()
    {
        var templates = new TemplateService(new TestAppLogger(), TestPaths.TemplatesDirectory);
        var template = templates.GetById("personnel-activity-report")!;
        var path = TestPaths.CreateTempFile();
        try
        {
            _builder.BuildAndSave(new GenerationRequest
            {
                Template = template,
                DateCalendar = DateCalendarKind.Jalali
            }, path);

            using var wb = new XLWorkbook(path);
            var sheet = wb.Worksheet("فعالیت پرسنل");

            sheet.Cell(1, 1).GetString().Should().Be("نام");
            sheet.Cell(1, 2).GetString().Should().Be("ساعت ورود");
            sheet.Cell(1, 3).GetString().Should().Be("ساعت خروج");
            sheet.Cell(1, 4).GetString().Should().Be("شرح کار");
            sheet.Cell(1, 1).GetString().Should().Be("نام");
            sheet.Cell(1, 2).GetString().Should().Be("ساعت ورود");
            sheet.Cell(2, 1).IsEmpty().Should().BeTrue();
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    [Fact]
    public void BuildAndSave_ProductionTemplate_Succeeds()
    {
        var templates = new TemplateService(new TestAppLogger(), TestPaths.TemplatesDirectory);
        var template = templates.GetById("personnel-activity-report")!;
        var path = TestPaths.CreateTempFile();
        try
        {
            var act = () => _builder.BuildAndSave(new GenerationRequest
            {
                Template = template,
                DateCalendar = DateCalendarKind.Jalali
            }, path);

            act.Should().NotThrow();
            using var wb = new XLWorkbook(path);
            wb.Worksheet("فعالیت پرسنل").Cell(1, 1).GetString().Should().Be("نام");
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }
}
