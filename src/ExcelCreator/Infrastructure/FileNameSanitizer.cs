using System.IO;

namespace ExcelCreator.Infrastructure;

public static class FileNameSanitizer
{
    public static string Sanitize(string name)
    {
        var invalid = Path.GetInvalidFileNameChars();
        return new string(name.Select(c => invalid.Contains(c) ? '_' : c).ToArray());
    }
}
