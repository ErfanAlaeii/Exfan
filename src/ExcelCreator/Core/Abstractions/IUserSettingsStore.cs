using ExcelCreator.Core.Models;

namespace ExcelCreator.Core.Abstractions;

public interface IUserSettingsStore
{
    AppSettings Load();
    void Save(AppSettings settings);
}
