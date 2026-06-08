using System.Globalization;
using System.Text;
using ExcelCreator.Core.Models;

namespace ExcelCreator.Application.Common;

public static class DateCalendarService
{
    private static readonly PersianCalendar Persian = new();

    public static string FormatDisplay(string? rawValue, DateCalendarKind kind, bool persianDigits = true)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
            return string.Empty;

        return TryParseToGregorian(rawValue, out var date)
            ? Format(date, kind, persianDigits)
            : rawValue;
    }

    public static string Format(DateTime date, DateCalendarKind kind, bool persianDigits = true)
    {
        string text;
        if (kind == DateCalendarKind.Gregorian)
        {
            text = date.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
        }
        else
        {
            var y = Persian.GetYear(date);
            var m = Persian.GetMonth(date);
            var d = Persian.GetDayOfMonth(date);
            text = $"{y:0000}/{m:00}/{d:00}";
        }

        return persianDigits ? ToPersianDigits(text) : text;
    }

    public static string FormatDateTime(DateTime dateTime, DateCalendarKind kind, bool persianDigits = true)
    {
        var datePart = Format(dateTime, kind, persianDigits);
        var timePart = dateTime.ToString("HH:mm", CultureInfo.InvariantCulture);
        if (persianDigits)
            timePart = ToPersianDigits(timePart);

        return $"{datePart} {timePart}";
    }

    public static bool TryParseToGregorian(string input, out DateTime result)
    {
        input = NormalizeDigits(input.Trim());
        if (string.IsNullOrEmpty(input))
        {
            result = default;
            return false;
        }

        if (TryParseSlashSeparated(input, out result))
            return true;

        if (DateTime.TryParse(input, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            return true;

        return DateTime.TryParse(input, new CultureInfo("fa-IR"), DateTimeStyles.None, out result);
    }

    private static bool TryParseSlashSeparated(string input, out DateTime result)
    {
        result = default;
        var normalized = input.Replace('-', '/');
        var parts = normalized.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 3)
            return false;

        if (!int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var year))
            return false;
        if (!int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var month))
            return false;
        if (!int.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out var day))
            return false;

        // سال ۱۲۰۰–۱۵۰۰ → شمسی؛ سال ۱۹۰۰–۲۱۰۰ → میلادی
        if (year is >= 1200 and <= 1500)
            return TryJalaliParts(year, month, day, out result);

        if (year is >= 1900 and <= 2100)
            return TryGregorianParts(year, month, day, out result);

        return false;
    }

    private static bool TryJalaliParts(int year, int month, int day, out DateTime result)
    {
        try
        {
            result = Persian.ToDateTime(year, month, day, 0, 0, 0, 0);
            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
            result = default;
            return false;
        }
    }

    public static void GetJalaliParts(DateTime date, out int year, out int month, out int day)
    {
        year = Persian.GetYear(date);
        month = Persian.GetMonth(date);
        day = Persian.GetDayOfMonth(date);
    }

    public static bool TryFromJalaliParts(int year, int month, int day, out DateTime result) =>
        TryJalaliParts(year, month, day, out result);

    public static int GetJalaliDaysInMonth(int year, int month) =>
        Persian.GetDaysInMonth(year, month);

    public static string FormatGregorianSearchValue(DateTime date) =>
        date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    public static int GetJalaliGridColumn(DayOfWeek dayOfWeek) =>
        dayOfWeek switch
        {
            DayOfWeek.Saturday => 0,
            DayOfWeek.Sunday => 1,
            DayOfWeek.Monday => 2,
            DayOfWeek.Tuesday => 3,
            DayOfWeek.Wednesday => 4,
            DayOfWeek.Thursday => 5,
            DayOfWeek.Friday => 6,
            _ => 0
        };

    private static bool TryGregorianParts(int year, int month, int day, out DateTime result)
    {
        try
        {
            result = new DateTime(year, month, day);
            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
            result = default;
            return false;
        }
    }

    public static string GetExcelDateFormat(DateCalendarKind kind) =>
        kind == DateCalendarKind.Gregorian ? "yyyy/mm/dd" : "@";

    public static string GetCalendarLabel(DateCalendarKind kind) =>
        kind == DateCalendarKind.Gregorian
            ? Localization.PersianStrings.CalendarGregorian
            : Localization.PersianStrings.CalendarJalali;

    public static DateCalendarKind ParseKind(string? value) =>
        string.Equals(value, nameof(DateCalendarKind.Gregorian), StringComparison.OrdinalIgnoreCase)
            ? DateCalendarKind.Gregorian
            : DateCalendarKind.Jalali;

    public static string ToStorageValue(DateCalendarKind kind) =>
        kind == DateCalendarKind.Gregorian ? "Gregorian" : "Jalali";

    public static string NormalizeDigits(string input)
    {
        var sb = new StringBuilder(input.Length);
        foreach (var ch in input)
        {
            sb.Append(ch switch
            {
                '۰' => '0',
                '۱' => '1',
                '۲' => '2',
                '۳' => '3',
                '۴' => '4',
                '۵' => '5',
                '۶' => '6',
                '۷' => '7',
                '۸' => '8',
                '۹' => '9',
                '٠' => '0',
                '١' => '1',
                '٢' => '2',
                '٣' => '3',
                '٤' => '4',
                '٥' => '5',
                '٦' => '6',
                '٧' => '7',
                '٨' => '8',
                '٩' => '9',
                _ => ch
            });
        }

        return sb.ToString();
    }

    public static string ToPersianDigits(string text) =>
        text
            .Replace('0', '۰').Replace('1', '۱').Replace('2', '۲').Replace('3', '۳')
            .Replace('4', '۴').Replace('5', '۵').Replace('6', '۶').Replace('7', '۷')
            .Replace('8', '۸').Replace('9', '۹');
}
