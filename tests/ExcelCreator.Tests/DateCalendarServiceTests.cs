using ExcelCreator.Models;
using ExcelCreator.Services;
using FluentAssertions;

namespace ExcelCreator.Tests;

public class DateCalendarServiceTests
{
    private static readonly DateTime SampleGregorian = new(2026, 6, 2);

    [Theory]
    [InlineData("2026-06-02")]
    [InlineData("2026/06/02")]
    [InlineData("۱۴۰۵/۰۳/۱۲")] // 1405/03/12 Jalali ≈ 2026-06-02
    public void TryParseToGregorian_AcceptsCommonFormats(string input)
    {
        var ok = DateCalendarService.TryParseToGregorian(input, out var date);

        ok.Should().BeTrue();
        date.Year.Should().Be(2026);
        date.Month.Should().Be(6);
        date.Day.Should().Be(2);
    }

    [Fact]
    public void Format_Gregorian_UsesSlashSeparators()
    {
        var text = DateCalendarService.Format(SampleGregorian, DateCalendarKind.Gregorian, persianDigits: false);

        text.Should().Be("2026/06/02");
    }

    [Fact]
    public void Format_Jalali_ConvertsFromGregorianDate()
    {
        var text = DateCalendarService.Format(SampleGregorian, DateCalendarKind.Jalali, persianDigits: false);

        text.Should().Be("1405/03/12");
    }

    [Fact]
    public void Format_Jalali_WithPersianDigits()
    {
        var text = DateCalendarService.Format(SampleGregorian, DateCalendarKind.Jalali);

        text.Should().StartWith("۱۴۰۵");
    }

    [Fact]
    public void FormatDateTime_Jalali_UsesJalaliDateAndPersianTimeDigits()
    {
        var dateTime = new DateTime(2026, 6, 5, 13, 43, 0);

        var text = DateCalendarService.FormatDateTime(dateTime, DateCalendarKind.Jalali);

        text.Should().StartWith("۱۴۰۵");
        text.Should().EndWith("۱۳:۴۳");
    }

    [Fact]
    public void FormatDateTime_Gregorian_UsesGregorianDate()
    {
        var dateTime = new DateTime(2026, 6, 5, 13, 43, 0);

        var text = DateCalendarService.FormatDateTime(dateTime, DateCalendarKind.Gregorian, persianDigits: false);

        text.Should().Be("2026/06/05 13:43");
    }

    [Fact]
    public void FormatDisplay_ConvertsIsoSampleToJalali()
    {
        var display = DateCalendarService.FormatDisplay("2026-06-02", DateCalendarKind.Jalali, persianDigits: false);

        display.Should().Be("1405/03/12");
    }

    [Theory]
    [InlineData("۰۱۲۳", "0123")]
    [InlineData("١٢٣", "123")]
    public void NormalizeDigits_ConvertsPersianAndArabicIndic(string input, string expected)
    {
        DateCalendarService.NormalizeDigits(input).Should().Be(expected);
    }

    [Theory]
    [InlineData("Gregorian", DateCalendarKind.Gregorian)]
    [InlineData("gregorian", DateCalendarKind.Gregorian)]
    [InlineData(null, DateCalendarKind.Jalali)]
    [InlineData("Jalali", DateCalendarKind.Jalali)]
    public void ParseKind_MapsStorageValues(string? value, DateCalendarKind expected)
    {
        DateCalendarService.ParseKind(value).Should().Be(expected);
    }

    [Theory]
    [InlineData(DateCalendarKind.Gregorian, "yyyy/mm/dd")]
    [InlineData(DateCalendarKind.Jalali, "@")]
    public void GetExcelDateFormat_ReturnsExpected(DateCalendarKind kind, string format)
    {
        DateCalendarService.GetExcelDateFormat(kind).Should().Be(format);
    }

    [Fact]
    public void TryParseToGregorian_RejectsInvalidJalaliDate()
    {
        var ok = DateCalendarService.TryParseToGregorian("1405/13/40", out _);

        ok.Should().BeFalse();
    }
}
