using ExcelCreator.Models;
using ExcelCreator.Services;
using FluentAssertions;
using ExcelCreator.Tests.Helpers;

namespace ExcelCreator.Tests.Performance;

/// <summary>
/// Performance gates — fail if generation or loading regresses beyond thresholds.
/// </summary>
[Trait("Category", "Performance")]
public class WorkbookBuilderPerformanceTests
{
    private static readonly ExcelWorkbookBuilder Builder = new();

    [Fact]
    public void LoadAllTemplates_CompletesWithin500ms()
    {
        PerformanceAssert.CompletesWithin(
            () =>
            {
                var service = new TemplateService(new TestAppLogger(), TestPaths.TemplatesDirectory);
                var count = service.LoadAll().Count;
                count.Should().BeGreaterOrEqualTo(1);
            },
            TimeSpan.FromMilliseconds(500),
            "LoadAll templates");
    }

    [Fact]
    public void BuildMinimalWorkbook_CompletesWithin2Seconds()
    {
        var template = new TemplateService(new TestAppLogger(), TestPaths.FixturesDirectory).GetById("test-minimal")!;

        PerformanceAssert.CompletesWithin(
            () =>
            {
                var path = TestPaths.CreateTempFile();
                try
                {
                    Builder.BuildAndSave(new GenerationRequest
                    {
                        Template = template,
                        DateCalendar = DateCalendarKind.Jalali
                    }, path);
                }
                finally
                {
                    if (File.Exists(path)) File.Delete(path);
                }
            },
            TimeSpan.FromSeconds(2),
            "Build minimal workbook");
    }

    [Fact]
    public void BuildPersonnelActivityReport_CompletesWithin3Seconds()
    {
        var template = new TemplateService(new TestAppLogger(), TestPaths.TemplatesDirectory)
            .GetById("personnel-activity-report")!;

        PerformanceAssert.CompletesWithin(
            () =>
            {
                var path = TestPaths.CreateTempFile();
                try
                {
                    Builder.BuildAndSave(new GenerationRequest
                    {
                        Template = template,
                        DateCalendar = DateCalendarKind.Jalali
                    }, path);
                }
                finally
                {
                    if (File.Exists(path)) File.Delete(path);
                }
            },
            TimeSpan.FromSeconds(3),
            "Build personnel activity report");
    }

    [Fact]
    public void BuildTenWorkbooks_AverageUnder2SecondsEach()
    {
        var template = new TemplateService(new TestAppLogger(), TestPaths.FixturesDirectory).GetById("test-minimal")!;
        var sw = System.Diagnostics.Stopwatch.StartNew();

        for (var i = 0; i < 10; i++)
        {
            var path = TestPaths.CreateTempFile();
            try
            {
                Builder.BuildAndSave(new GenerationRequest
                {
                    Template = template,
                    DateCalendar = DateCalendarKind.Jalali
                }, path);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        sw.Stop();
        var averageMs = sw.Elapsed.TotalMilliseconds / 10;
        averageMs.Should().BeLessThan(2000,
            $"average build time was {averageMs:F0} ms over 10 iterations");
    }

    [Fact]
    public void LargeUserRowCount_BuildCompletesWithin5Seconds()
    {
        var template = new TemplateService(new TestAppLogger(), TestPaths.FixturesDirectory).GetById("test-minimal")!;
        var rows = Enumerable.Range(1, 500)
            .Select(i => new List<string> { $"row-{i}", "2026-06-02", "1000" })
            .ToList();

        PerformanceAssert.CompletesWithin(
            () =>
            {
                var path = TestPaths.CreateTempFile();
                try
                {
                    Builder.BuildAndSave(new GenerationRequest
                    {
                        Template = template,
                        UserRows = rows,
                        DateCalendar = DateCalendarKind.Gregorian
                    }, path);
                }
                finally
                {
                    if (File.Exists(path)) File.Delete(path);
                }
            },
            TimeSpan.FromSeconds(5),
            "Build workbook with 500 user rows");
    }
}
