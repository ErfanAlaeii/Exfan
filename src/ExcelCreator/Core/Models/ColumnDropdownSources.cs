namespace ExcelCreator.Core.Models;

public static class ColumnDropdownSources
{
    public const string Personnel = "personnel";

    public static IReadOnlyList<string> All { get; } = [Personnel];

    public static bool IsKnown(string? source) =>
        All.Any(value => string.Equals(value, source, StringComparison.OrdinalIgnoreCase));

    public static bool IsPersonnel(string? source) =>
        string.Equals(source, Personnel, StringComparison.OrdinalIgnoreCase);
}
