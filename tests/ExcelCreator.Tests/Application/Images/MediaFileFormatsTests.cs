using ExcelCreator.Application.Images;
using FluentAssertions;

namespace ExcelCreator.Tests.Application.Images;

public class MediaFileFormatsTests
{
    [Theory]
    [InlineData("photo.jpg", true, true)]
    [InlineData("photo.JPEG", true, true)]
    [InlineData("scan.tiff", true, true)]
    [InlineData("doc.pdf", true, false)]
    [InlineData("photo.heic", true, false)]
    [InlineData("notes.txt", false, false)]
    public void IsAllowed_and_IsRasterImage(string fileName, bool allowed, bool raster)
    {
        MediaFileFormats.IsAllowed(fileName).Should().Be(allowed);
        MediaFileFormats.IsRasterImage(fileName).Should().Be(raster);
    }

    [Fact]
    public void FormatGridValue_UsesPdfIconForPdf()
    {
        var path = Path.Combine(Path.GetTempPath(), $"Exfan_Test_{Guid.NewGuid():N}.pdf");
        File.WriteAllText(path, "%PDF-1.0");
        try
        {
            MediaFileFormats.FormatGridValue(path).Should().StartWith("📄");
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void BuildOpenFileDialogFilter_IncludesPdfAndJpg()
    {
        var filter = MediaFileFormats.BuildOpenFileDialogFilter();
        filter.Should().Contain("*.pdf");
        filter.Should().Contain("*.jpg");
    }
}
