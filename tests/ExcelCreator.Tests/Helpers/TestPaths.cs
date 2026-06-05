namespace ExcelCreator.Tests.Helpers;

internal static class TestPaths
{
    public static string BaseDirectory => AppContext.BaseDirectory;

    public static string TemplatesDirectory => Path.Combine(BaseDirectory, "Templates");

    public static string FixturesDirectory => Path.Combine(BaseDirectory, "Fixtures");

    public static string CreateTempFile(string extension = ".xlsx") =>
        Path.Combine(Path.GetTempPath(), $"ExcelCreator_Test_{Guid.NewGuid():N}{extension}");
}
