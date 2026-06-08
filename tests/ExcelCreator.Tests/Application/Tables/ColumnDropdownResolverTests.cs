using ExcelCreator.Application.Personnel;
using ExcelCreator.Application.Tables;
using ExcelCreator.Core.Models;
using FluentAssertions;

namespace ExcelCreator.Tests.Application.Tables;

public class ColumnDropdownResolverTests : IDisposable
{
    private readonly string _tempFile;
    private readonly PersonnelService _personnel;

    public ColumnDropdownResolverTests()
    {
        _tempFile = Path.Combine(Path.GetTempPath(), $"exfan-personnel-{Guid.NewGuid():N}.json");
        _personnel = new PersonnelService(_tempFile);
    }

    public void Dispose()
    {
        if (File.Exists(_tempFile))
            File.Delete(_tempFile);
    }

    [Fact]
    public void ResolveColumns_FillsPersonnelDropdownValues()
    {
        _personnel.Add("علی");
        _personnel.Add("رضا");

        var columns = new List<ColumnSpec>
        {
            new() { Header = "نام", Type = ColumnTypes.Text, DropdownSource = ColumnDropdownSources.Personnel },
            new() { Header = "شرح", Type = ColumnTypes.Text }
        };

        var resolved = ColumnDropdownResolver.ResolveColumns(columns, _personnel);

        resolved[0].DropdownValues.Should().BeEquivalentTo(["علی", "رضا"]);
        resolved[1].DropdownValues.Should().BeNull();
    }

    [Fact]
    public void RequiresPersonnel_ReturnsTrueWhenSourceSet()
    {
        var columns = new List<ColumnSpec>
        {
            new() { Header = "نام", Type = ColumnTypes.Text, DropdownSource = ColumnDropdownSources.Personnel }
        };

        ColumnDropdownResolver.RequiresPersonnel(columns).Should().BeTrue();
    }
}
