namespace ExcelCreator.Core.Abstractions;

public interface IMediaFileOpener
{
    bool TryOpen(string? filePath, object? ownerWindow = null);
}
