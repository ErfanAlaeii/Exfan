using System.IO;

namespace ExcelCreator.Application.Images;

public static class MediaFileFormats
{
    private static readonly string[] AllowedExtensionList =
    [
        ".jpg", ".jpeg", ".jfif",
        ".png", ".webp", ".bmp", ".gif",
        ".tif", ".tiff",
        ".ico",
        ".heic", ".heif",
        ".pdf"
    ];

    private static readonly HashSet<string> AllowedExtensions =
        new(AllowedExtensionList, StringComparer.OrdinalIgnoreCase);

    private static readonly HashSet<string> RasterImageExtensions =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".jfif",
            ".png", ".webp", ".bmp", ".gif",
            ".tif", ".tiff",
            ".ico"
        };

    public static IReadOnlyCollection<string> Extensions => AllowedExtensionList;

    public static bool IsAllowed(string? path) =>
        !string.IsNullOrWhiteSpace(path) &&
        AllowedExtensions.Contains(Path.GetExtension(path.Trim()));

    public static bool IsRasterImage(string? path) =>
        !string.IsNullOrWhiteSpace(path) &&
        RasterImageExtensions.Contains(Path.GetExtension(path.Trim()));

    public static bool IsPdf(string? path) =>
        string.Equals(Path.GetExtension(path?.Trim()), ".pdf", StringComparison.OrdinalIgnoreCase);

    public static string BuildOpenFileDialogFilter()
    {
        var patterns = string.Join(";", AllowedExtensionList.Select(ext => $"*{ext}"));
        return $"تصاویر و PDF ({patterns})|{patterns}";
    }

    public static string FormatGridValue(string? storedPath)
    {
        if (string.IsNullOrWhiteSpace(storedPath))
            return string.Empty;

        var trimmed = storedPath.Trim();
        var fileName = Path.GetFileName(trimmed);
        if (!File.Exists(trimmed))
            return fileName;

        var prefix = IsPdf(trimmed) ? "📄" : "🖼";
        return $"{prefix} {fileName}";
    }
}
