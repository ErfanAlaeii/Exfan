using ExcelCreator.Models;
using ExcelCreator.Services;
using FluentAssertions;

namespace ExcelCreator.Tests;

public class SavedTableServiceTests : IDisposable
{
    private readonly string _tempFile;
    private readonly SavedTableService _service;

    private static readonly SheetSpec TestSheet = new()
    {
        Name = "داده",
        Columns =
        [
            new ColumnSpec { Header = "نام", Type = ColumnTypes.Text },
            new ColumnSpec { Header = "ساعت ورود", Type = ColumnTypes.Text },
            new ColumnSpec { Header = "ساعت خروج", Type = ColumnTypes.Text },
            new ColumnSpec { Header = "شرح کار", Type = ColumnTypes.Text }
        ]
    };

    public SavedTableServiceTests()
    {
        _tempFile = Path.Combine(Path.GetTempPath(), $"exfan-tables-{Guid.NewGuid():N}.json");
        _service = new SavedTableService(_tempFile);
    }

    public void Dispose()
    {
        if (File.Exists(_tempFile))
            File.Delete(_tempFile);
    }

    private static TemplateDefinition CreateTemplate(string id, int version = 1) => new()
    {
        Id = id,
        Title = "الگوی آزمایشی",
        Version = version,
        Workbook = new WorkbookSpec { Sheets = [TestSheet] }
    };

    [Fact]
    public void SaveAndGetByTemplate_ReturnsSavedTable()
    {
        var template = CreateTemplate("personnel-activity-report");
        var table = new SavedTable
        {
            TemplateId = template.Id,
            Name = "گزارش هفته",
            DateCalendar = DateCalendarKind.Jalali,
            Rows = [TableRow.CreateNew(["علی", "08:00", "17:00", "کار اداری"])]
        };

        _service.Save(table, template);

        var loaded = _service.GetByTemplate("personnel-activity-report");
        loaded.Should().HaveCount(1);
        loaded[0].Name.Should().Be("گزارش هفته");
        loaded[0].Rows[0].Values[0].Should().Be("علی");
        loaded[0].Rows[0].CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        loaded[0].ColumnHeaders.Should().Equal("نام", "ساعت ورود", "ساعت خروج", "شرح کار");
        loaded[0].TemplateVersion.Should().Be(1);
    }

    [Fact]
    public void Load_MigratesLegacyRowArrays()
    {
        var legacyJson = """
            {
              "tables": [
                {
                  "id": "legacy1",
                  "templateId": "personnel-activity-report",
                  "name": "قدیمی",
                  "dateCalendar": 1,
                  "rows": [["علی", "08:00", "17:00", "کار اداری"]]
                }
              ]
            }
            """;
        File.WriteAllText(_tempFile, legacyJson);

        var loaded = _service.GetByTemplate("personnel-activity-report");
        loaded.Should().HaveCount(1);
        loaded[0].Rows.Should().HaveCount(1);
        loaded[0].Rows[0].Values.Should().Equal("علی", "08:00", "17:00", "کار اداری");
        loaded[0].Rows[0].CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void NameExistsForTemplate_DetectsDuplicate()
    {
        var template = CreateTemplate("test-template");
        _service.Save(new SavedTable
        {
            TemplateId = template.Id,
            Name = "جدول ۱"
        }, template);

        _service.NameExistsForTemplate("test-template", "جدول ۱").Should().BeTrue();
        _service.NameExistsForTemplate("test-template", "جدول ۲").Should().BeFalse();
    }

    [Fact]
    public void Delete_RemovesTable()
    {
        var template = CreateTemplate("test-template");
        var table = new SavedTable
        {
            TemplateId = template.Id,
            Name = "موقت"
        };
        _service.Save(table, template);

        _service.Delete(table.Id).Should().BeTrue();
        _service.GetByTemplate("test-template").Should().BeEmpty();
    }

    [Fact]
    public void Save_RejectsRowWithTooManyColumns()
    {
        var template = CreateTemplate("test");
        var table = new SavedTable
        {
            TemplateId = template.Id,
            Name = "bad",
            Rows = [TableRow.CreateNew(["a", "b", "c", "d", "extra"])]
        };

        var act = () => _service.Save(table, template);
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Save_RejectsTemplateVersionMismatch()
    {
        var template = CreateTemplate("test", version: 2);
        var table = new SavedTable
        {
            TemplateId = template.Id,
            TemplateVersion = 1,
            Name = "جدول",
            Rows = [TableRow.CreateNew(["a", "b", "c", "d"])]
        };

        var act = () => _service.Save(table, template);
        act.Should().Throw<InvalidOperationException>();
    }
}
