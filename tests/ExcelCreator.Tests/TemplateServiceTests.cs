using ExcelCreator.Models;
using ExcelCreator.Services;
using ExcelCreator.Validation;
using FluentAssertions;
using ExcelCreator.Tests.Helpers;

namespace ExcelCreator.Tests;

public class TemplateServiceTests
{
    private static TemplateService CreateService(string directory) =>
        new(new TestAppLogger(), directory);

    [Fact]
    public void LoadAll_FromFixtures_LoadsMinimalTemplate()
    {
        var service = CreateService(TestPaths.FixturesDirectory);

        var templates = service.LoadAll();

        templates.Should().ContainSingle(t => t.Id == "test-minimal");
        templates[0].Workbook.Sheets.Should().NotBeEmpty();
        templates[0].Version.Should().Be(1);
    }

    [Fact]
    public void LoadAll_FromShippedTemplates_LoadsPersonnelActivityReport()
    {
        Directory.Exists(TestPaths.TemplatesDirectory).Should().BeTrue(
            "test output must include Templates from src/ExcelCreator/Templates");

        var service = CreateService(TestPaths.TemplatesDirectory);

        var templates = service.LoadAll();

        templates.Should().ContainSingle();
        templates[0].Id.Should().Be("personnel-activity-report");
        templates[0].Title.Should().Be("گزارش فعالیت پرسنل");
    }

    [Fact]
    public void GetById_IsCaseInsensitive()
    {
        var service = CreateService(TestPaths.TemplatesDirectory);

        var template = service.GetById("PERSONNEL-ACTIVITY-REPORT");

        template.Should().NotBeNull();
        template!.Id.Should().Be("personnel-activity-report");
    }

    [Fact]
    public void LoadAll_SkipsInvalidJsonFiles()
    {
        var dir = Path.Combine(Path.GetTempPath(), $"ExcelCreator_TplTest_{Guid.NewGuid():N}");
        Directory.CreateDirectory(dir);
        try
        {
            File.WriteAllText(Path.Combine(dir, "bad.json"), "{ not valid json");
            File.Copy(
                Path.Combine(TestPaths.FixturesDirectory, "minimal-template.json"),
                Path.Combine(dir, "good.json"));

            var service = CreateService(dir);
            var templates = service.LoadAll();

            templates.Should().ContainSingle(t => t.Id == "test-minimal");
        }
        finally
        {
            Directory.Delete(dir, recursive: true);
        }
    }
}

public class TemplateValidatorTests
{
    [Fact]
    public void Validate_RejectsUnknownColumnType()
    {
        var template = new TemplateDefinition
        {
            Id = "bad",
            Workbook = new WorkbookSpec
            {
                Sheets =
                [
                    new SheetSpec
                    {
                        Name = "داده",
                        Columns = [new ColumnSpec { Header = "x", Type = "unknown" }]
                    }
                ]
            }
        };

        TemplateValidator.Validate(template).IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_AcceptsMinimalTemplate()
    {
        var template = new TemplateDefinition
        {
            Id = "ok",
            Title = "الگوی معتبر",
            Workbook = new WorkbookSpec
            {
                Sheets =
                [
                    new SheetSpec
                    {
                        Name = "داده",
                        Columns = [new ColumnSpec { Header = "نام", Type = ColumnTypes.Text }]
                    }
                ]
            }
        };

        TemplateValidator.Validate(template).IsValid.Should().BeTrue();
    }
}
