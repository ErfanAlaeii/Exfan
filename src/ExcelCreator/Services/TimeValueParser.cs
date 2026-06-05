using System.Globalization;

namespace ExcelCreator.Services;

public static class TimeValueParser
{
    public static bool TryParseTime(string? value, out TimeSpan time)
    {
        time = default;
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var normalized = DateCalendarService.NormalizeDigits(value.Trim());
        var parts = normalized.Split(':', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 1)
        {
            if (!int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var hoursOnly))
                return false;
            if (hoursOnly is < 0 or > 23)
                return false;

            time = new TimeSpan(hoursOnly, 0, 0);
            return true;
        }

        if (parts.Length != 2)
            return false;

        if (!int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var hours))
            return false;
        if (!int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var minutes))
            return false;
        if (hours is < 0 or > 23 || minutes is < 0 or > 59)
            return false;

        time = new TimeSpan(hours, minutes, 0);
        return true;
    }

    public static TimeSpan CalculateDuration(TimeSpan start, TimeSpan end)
    {
        var duration = end - start;
        return duration < TimeSpan.Zero ? duration + TimeSpan.FromHours(24) : duration;
    }

    public static string FormatDuration(TimeSpan duration, bool usePersianDigits = true)
    {
        var totalMinutes = (int)duration.TotalMinutes;
        var hours = totalMinutes / 60;
        var minutes = totalMinutes % 60;
        var text = $"{hours}:{minutes:00}";
        return usePersianDigits ? DateCalendarService.ToPersianDigits(text) : text;
    }
}
