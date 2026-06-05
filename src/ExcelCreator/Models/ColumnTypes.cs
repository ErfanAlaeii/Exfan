namespace ExcelCreator.Models;

public static class ColumnTypes
{
    public const string Text = "text";
    public const string Number = "number";
    public const string Date = "date";
    public const string Currency = "currency";

    public static IReadOnlyList<string> All { get; } =
        [Text, Number, Date, Currency];

    public static bool IsKnown(string? type) =>
        All.Any(t => string.Equals(t, type, StringComparison.OrdinalIgnoreCase));

    public static bool IsDate(string? type) =>
        string.Equals(type, Date, StringComparison.OrdinalIgnoreCase);

    public static bool IsNumber(string? type) =>
        string.Equals(type, Number, StringComparison.OrdinalIgnoreCase);

    public static bool IsCurrency(string? type) =>
        string.Equals(type, Currency, StringComparison.OrdinalIgnoreCase);

    public static bool IsNumeric(string? type) =>
        IsNumber(type) || IsCurrency(type);
}
