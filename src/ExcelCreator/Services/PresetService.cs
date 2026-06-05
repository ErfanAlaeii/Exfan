using ExcelCreator.Abstractions;
using ExcelCreator.Infrastructure;
using ExcelCreator.Models;

namespace ExcelCreator.Services;

public sealed class PresetService : IUserSettingsStore
{
    private readonly AtomicJsonStore<AppSettings> _store;

    public PresetService(string? settingsFilePath = null)
    {
        var path = settingsFilePath ?? AppPaths.SettingsFile;
        _store = new AtomicJsonStore<AppSettings>(path, JsonDefaults.Storage, "تنظیمات");
    }

    public AppSettings Load() => _store.LoadOrDefault(() => new AppSettings());

    public void Save(AppSettings settings) => _store.Save(settings);
}

public sealed class AppSettings
{
    public string? DefaultSaveFolder { get; set; }
    public bool OpenAfterCreate { get; set; } = true;
    public string? LastTemplateId { get; set; }
    public DateCalendarKind DateCalendar { get; set; } = DateCalendarKind.Jalali;
}
