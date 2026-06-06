using System.Globalization;
using ExcelCreator.Application.Common;

namespace ExcelCreator.Application.Calculations;

public static class CalculationFormatter
{
    public static string FormatNumber(decimal value, bool usePersianDigits = true)
    {
        var text = value.ToString("#,##0.##", CultureInfo.InvariantCulture);
        return usePersianDigits ? DateCalendarService.ToPersianDigits(text) : text;
    }

    public static string FormatInteger(int value, bool usePersianDigits = true)
    {
        var text = value.ToString(CultureInfo.InvariantCulture);
        return usePersianDigits ? DateCalendarService.ToPersianDigits(text) : text;
    }

    public static string FormatPercent(decimal value, bool usePersianDigits = true)
    {
        var text = value.ToString("0.##", CultureInfo.InvariantCulture) + "%";
        return usePersianDigits ? DateCalendarService.ToPersianDigits(text) : text;
    }
}
