using ExcelCreator.Services;

namespace ExcelCreator.Abstractions;

public interface IUserSettingsStore
{
    AppSettings Load();
    void Save(AppSettings settings);
}
