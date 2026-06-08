using System.IO;
using ExcelCreator.Application.Images;
using ExcelCreator.Infrastructure.Paths;
using FluentAssertions;

namespace ExcelCreator.Tests.Application.Images;

public class ImageStorageServiceTests : IDisposable
{
    private readonly ImageStorageService _service = new();
    private readonly List<string> _cleanupPaths = [];

    [Fact]
    public void ImportFromFile_CopiesToManagedDirectory()
    {
        var source = CreateTempImage(".png");

        var stored = _service.ImportFromFile(source);

        stored.Should().NotBe(Path.GetFullPath(source));
        File.Exists(stored).Should().BeTrue();
        _service.IsManagedPath(stored).Should().BeTrue();
        _service.GetDisplayLabel(stored).Should().Be(Path.GetFileName(stored));
        _cleanupPaths.Add(stored);
    }

    [Fact]
    public void ImportFromFile_ReturnsSamePathWhenAlreadyManaged()
    {
        var source = CreateTempImage(".jpg");
        var stored = _service.ImportFromFile(source);
        _cleanupPaths.Add(stored);

        var secondImport = _service.ImportFromFile(stored);

        secondImport.Should().Be(Path.GetFullPath(stored));
    }

    [Fact]
    public void Exists_ReturnsFalseForMissingFile()
    {
        _service.Exists(Path.Combine(Path.GetTempPath(), $"missing_{Guid.NewGuid():N}.png")).Should().BeFalse();
    }

    [Fact]
    public void ImportFromFile_AcceptsPdf()
    {
        var source = Path.Combine(Path.GetTempPath(), $"Exfan_ImageTest_{Guid.NewGuid():N}.pdf");
        File.WriteAllText(source, "%PDF-1.0\n%%EOF");
        _cleanupPaths.Add(source);

        var stored = _service.ImportFromFile(source);

        stored.Should().EndWith(".pdf");
        File.Exists(stored).Should().BeTrue();
        _cleanupPaths.Add(stored);
    }

    private string CreateTempImage(string extension)
    {
        var source = Path.Combine(Path.GetTempPath(), $"Exfan_ImageTest_{Guid.NewGuid():N}{extension}");
        File.WriteAllBytes(source, extension.Equals(".png", StringComparison.OrdinalIgnoreCase)
            ? [0x89, 0x50, 0x4E, 0x47]
            : [0xFF, 0xD8, 0xFF]);
        _cleanupPaths.Add(source);
        return source;
    }

    public void Dispose()
    {
        foreach (var path in _cleanupPaths)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch
            {
                // Best effort cleanup for temp and managed test images.
            }
        }
    }
}
