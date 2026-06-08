using ExcelCreator.Core.Models;
using ExcelCreator.Application.Common;
using ExcelCreator.Core.Validation;
using FluentAssertions;

namespace ExcelCreator.Tests.Core.Validation;

public class TableValidatorTests
{
    private static readonly SheetSpec Sheet = new()
    {
        Columns =
        [
            new ColumnSpec { Header = "نام", Type = ColumnTypes.Text },
            new ColumnSpec { Header = "تاریخ", Type = ColumnTypes.Date },
            new ColumnSpec { Header = "مبلغ", Type = ColumnTypes.Currency }
        ]
    };

    [Fact]
    public void ValidateName_RejectsEmpty()
    {
        TableValidator.ValidateName("  ").IsValid.Should().BeFalse();
    }

    [Fact]
    public void NormalizeRows_PadsMissingColumns()
    {
        var normalized = TableValidator.NormalizeRows(Sheet, [["علی"]]);
        normalized[0].Should().Equal("علی", string.Empty, string.Empty);
    }

    [Fact]
    public void NormalizeRows_PreservesCreatedAt()
    {
        var createdAt = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var normalized = TableValidator.NormalizeRows(Sheet, [TableRow.FromValues(["علی"], createdAt)]);
        normalized[0].Values.Should().Equal("علی", string.Empty, string.Empty);
        normalized[0].CreatedAt.Should().Be(createdAt);
    }

    [Fact]
    public void ValidateRows_RejectsEmptyRow()
    {
        TableValidator.ValidateRows(Sheet, [["", "", ""]]).IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateRows_RejectsInvalidDate()
    {
        TableValidator.ValidateRows(Sheet, [["علی", "not-a-date", "100"]], DateCalendarKind.Jalali)
            .IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateRows_AcceptsValidDate()
    {
        TableValidator.ValidateRows(Sheet, [["علی", "2026-06-02", "1000"]], DateCalendarKind.Gregorian)
            .IsValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateRows_RejectsMissingImageFile()
    {
        var imageSheet = new SheetSpec
        {
            Columns = [new ColumnSpec { Header = "تصویر", Type = ColumnTypes.Image }]
        };

        TableValidator.ValidateRows(imageSheet, [["C:\\missing\\photo.jpg"]]).IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateRows_AcceptsExistingImageFile()
    {
        var imageSheet = new SheetSpec
        {
            Columns = [new ColumnSpec { Header = "تصویر", Type = ColumnTypes.Image }]
        };
        var tempImage = Path.Combine(Path.GetTempPath(), $"Exfan_Test_{Guid.NewGuid():N}.png");
        File.WriteAllBytes(tempImage, [0x89, 0x50, 0x4E, 0x47]);

        TableValidator.ValidateRows(imageSheet, [[tempImage]]).IsValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateRows_RejectsUnsupportedMediaFormat()
    {
        var imageSheet = new SheetSpec
        {
            Columns = [new ColumnSpec { Header = "تصویر", Type = ColumnTypes.Image }]
        };
        var tempFile = Path.Combine(Path.GetTempPath(), $"Exfan_Test_{Guid.NewGuid():N}.txt");
        File.WriteAllText(tempFile, "test");

        TableValidator.ValidateRows(imageSheet, [[tempFile]]).IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateRows_AcceptsPdfFile()
    {
        var imageSheet = new SheetSpec
        {
            Columns = [new ColumnSpec { Header = "تصویر", Type = ColumnTypes.Image }]
        };
        var tempPdf = Path.Combine(Path.GetTempPath(), $"Exfan_Test_{Guid.NewGuid():N}.pdf");
        File.WriteAllText(tempPdf, "%PDF-1.0\n%%EOF");

        TableValidator.ValidateRows(imageSheet, [[tempPdf]]).IsValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateTemplateCompatibility_RejectsVersionMismatch()
    {
        var table = new SavedTable { TemplateId = "x", TemplateVersion = 1, ColumnHeaders = ["نام", "تاریخ", "مبلغ"] };
        var template = new TemplateDefinition { Id = "x", Version = 2 };

        TableValidator.ValidateTemplateCompatibility(table, template).IsValid.Should().BeFalse();
    }
}
