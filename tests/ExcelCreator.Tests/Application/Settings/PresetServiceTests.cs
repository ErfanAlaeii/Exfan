using ExcelCreator.Core.Models;
using ExcelCreator.Application.Settings;
using ExcelCreator.Application.Common;
using FluentAssertions;
using ExcelCreator.Tests.Helpers;

namespace ExcelCreator.Tests.Application.Settings;

public class PresetServiceTests
{
    [Fact]
    public void Load_WhenMissing_ReturnsDefaults()
    {
        var path = TestPaths.CreateTempFile(".json");
        try
        {
            var service = new PresetService(path);
            var settings = service.Load();

            settings.OpenAfterCreate.Should().BeTrue();
            settings.DateCalendar.Should().Be(DateCalendarKind.Jalali);
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    [Fact]
    public void SaveAndLoad_RoundTripsSettings()
    {
        var path = TestPaths.CreateTempFile(".json");
        try
        {
            var service = new PresetService(path);
            service.Save(new AppSettings
            {
                DateCalendar = DateCalendarKind.Gregorian,
                DefaultSaveFolder = @"C:\Reports",
                LastTemplateId = "timesheet",
                OpenAfterCreate = false
            });

            var loaded = service.Load();

            loaded.DateCalendar.Should().Be(DateCalendarKind.Gregorian);
            loaded.DefaultSaveFolder.Should().Be(@"C:\Reports");
            loaded.LastTemplateId.Should().Be("timesheet");
            loaded.OpenAfterCreate.Should().BeFalse();
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }
}
