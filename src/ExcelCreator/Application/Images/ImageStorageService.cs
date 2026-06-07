using System.IO;
using ExcelCreator.Core.Abstractions;
using ExcelCreator.Infrastructure.Paths;

namespace ExcelCreator.Application.Images;

public sealed class ImageStorageService : IImageStorageService
{
    public string ImportFromFile(string sourcePath)
    {
        if (string.IsNullOrWhiteSpace(sourcePath))
            throw new ArgumentException("مسیر فایل خالی است.", nameof(sourcePath));

        var fullSource = Path.GetFullPath(sourcePath.Trim());
        if (!File.Exists(fullSource))
            throw new FileNotFoundException("فایل یافت نشد.", fullSource);

        if (!MediaFileFormats.IsAllowed(fullSource))
            throw new InvalidOperationException("فرمت فایل پشتیبانی نمی‌شود.");

        if (IsManagedPath(fullSource))
            return fullSource;

        var extension = Path.GetExtension(fullSource).ToLowerInvariant();
        var targetPath = Path.Combine(AppPaths.ImagesDirectory, $"{Guid.NewGuid():N}{extension}");
        File.Copy(fullSource, targetPath, overwrite: false);
        return targetPath;
    }

    public bool Exists(string? storedPath) =>
        !string.IsNullOrWhiteSpace(storedPath) && File.Exists(storedPath.Trim());

    public bool IsManagedPath(string? storedPath)
    {
        if (string.IsNullOrWhiteSpace(storedPath))
            return false;

        try
        {
            var imagesRoot = Path.GetFullPath(AppPaths.ImagesDirectory);
            var candidate = Path.GetFullPath(storedPath.Trim());
            return candidate.StartsWith(imagesRoot, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    public string GetDisplayLabel(string? storedPath)
    {
        if (string.IsNullOrWhiteSpace(storedPath))
            return string.Empty;

        return Path.GetFileName(storedPath.Trim());
    }
}
