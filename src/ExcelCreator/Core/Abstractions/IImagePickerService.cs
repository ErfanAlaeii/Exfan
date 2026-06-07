namespace ExcelCreator.Core.Abstractions;

public interface IImagePickerService
{
    bool TryPickImage(object? owner, out string sourcePath);
}
