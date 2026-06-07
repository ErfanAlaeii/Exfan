namespace ExcelCreator.Application.Images;

public static class ImageDisplayHelper
{
    public static string FormatGridValue(string? storedPath) =>
        MediaFileFormats.FormatGridValue(storedPath);
}
