using ExcelCreator.Core.Abstractions;
using ExcelCreator.Core.Models;
using ExcelCreator.Infrastructure.Persistence;
using ExcelCreator.Infrastructure.Paths;

namespace ExcelCreator.Application.Settings;

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
