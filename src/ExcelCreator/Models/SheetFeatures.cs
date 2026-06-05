namespace ExcelCreator.Models;

public static class SheetFeatures
{
    public const string FreezeHeader = "freeze_header";
    public const string TableStyle = "table_style";
    public const string AutoFilter = "auto_filter";
    public const string AlternateRows = "alternate_rows";

    public static IReadOnlyList<string> All { get; } =
        [FreezeHeader, TableStyle, AutoFilter, AlternateRows];

    public static bool IsKnown(string? feature) =>
        All.Any(f => string.Equals(f, feature, StringComparison.OrdinalIgnoreCase));
}
