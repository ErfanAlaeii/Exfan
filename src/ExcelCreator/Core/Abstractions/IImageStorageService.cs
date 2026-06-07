namespace ExcelCreator.Core.Abstractions;

public interface IImageStorageService
{
    string ImportFromFile(string sourcePath);

    bool Exists(string? storedPath);

    string GetDisplayLabel(string? storedPath);

    bool IsManagedPath(string? storedPath);
}
