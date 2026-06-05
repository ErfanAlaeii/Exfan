using System.Globalization;
using ExcelCreator.Models;

namespace ExcelCreator.Services;

public static class CalculationValueParser
{
    public static bool TryParseDecimal(string? value, out decimal result)
    {
        result = 0;
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return decimal.TryParse(
            DateCalendarService.NormalizeDigits(value.Trim()),
            NumberStyles.Any,
            CultureInfo.InvariantCulture,
            out result);
    }

    public static bool IsNumericColumn(ColumnSpec column) =>
        ColumnTypes.IsNumeric(column.Type);

    public static bool IsCalculableColumn(ColumnSpec column) =>
        ColumnTypes.IsNumeric(column.Type) ||
        column.Type.Equals(ColumnTypes.Text, StringComparison.OrdinalIgnoreCase);
}
