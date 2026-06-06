using ExcelCreator.Application.Calculations;
using FluentAssertions;

namespace ExcelCreator.Tests.Application.Calculations;

public class TimeValueParserTests
{
    [Theory]
    [InlineData("11", 11, 0)]
    [InlineData("8", 8, 0)]
    [InlineData("08:30", 8, 30)]
    [InlineData("17:00", 17, 0)]
    public void TryParseTime_AcceptsHourOnlyAndColonFormats(string input, int hours, int minutes)
    {
        TimeValueParser.TryParseTime(input, out var time).Should().BeTrue();
        time.Hours.Should().Be(hours);
        time.Minutes.Should().Be(minutes);
    }

    [Fact]
    public void CalculateDuration_WorksWithBareHourValues()
    {
        TimeValueParser.TryParseTime("11", out var start).Should().BeTrue();
        TimeValueParser.TryParseTime("18", out var end).Should().BeTrue();

        var duration = TimeValueParser.CalculateDuration(start, end);
        TimeValueParser.FormatDuration(duration, usePersianDigits: false).Should().Be("7:00");
    }
}
