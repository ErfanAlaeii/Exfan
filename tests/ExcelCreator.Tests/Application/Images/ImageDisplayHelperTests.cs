using ExcelCreator.Application.Images;
using FluentAssertions;

namespace ExcelCreator.Tests.Application.Images;

public class ImageDisplayHelperTests
{
    [Fact]
    public void FormatGridValue_ReturnsEmptyForBlankPath()
    {
        ImageDisplayHelper.FormatGridValue(null).Should().BeEmpty();
        ImageDisplayHelper.FormatGridValue("  ").Should().BeEmpty();
    }

    [Fact]
    public void FormatGridValue_ShowsFileNameWithoutIconWhenMissing()
    {
        var path = Path.Combine(Path.GetTempPath(), $"missing_{Guid.NewGuid():N}.jpg");
        ImageDisplayHelper.FormatGridValue(path).Should().Be(Path.GetFileName(path));
    }
}
