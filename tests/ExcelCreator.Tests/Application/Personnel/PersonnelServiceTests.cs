using ExcelCreator.Application.Personnel;
using ExcelCreator.Core.Models;
using FluentAssertions;

namespace ExcelCreator.Tests.Application.Personnel;

public class PersonnelServiceTests : IDisposable
{
    private readonly string _tempFile;
    private readonly PersonnelService _service;

    public PersonnelServiceTests()
    {
        _tempFile = Path.Combine(Path.GetTempPath(), $"exfan-personnel-{Guid.NewGuid():N}.json");
        _service = new PersonnelService(_tempFile);
    }

    public void Dispose()
    {
        if (File.Exists(_tempFile))
            File.Delete(_tempFile);
    }

    [Fact]
    public void AddAndGetAll_ReturnsSortedNames()
    {
        _service.Add("رضا");
        _service.Add("علی");

        _service.GetNames().Should().BeEquivalentTo(["علی", "رضا"]);
    }

    [Fact]
    public void Update_ChangesName()
    {
        var member = _service.Add("علی");
        _service.Update(member.Id, "علی‌رضا");

        _service.GetNames().Should().Equal("علی‌رضا");
    }

    [Fact]
    public void Delete_RemovesMember()
    {
        var member = _service.Add("علی");
        _service.Delete(member.Id).Should().BeTrue();

        _service.GetAll().Should().BeEmpty();
    }

    [Fact]
    public void Add_DuplicateName_Throws()
    {
        _service.Add("علی");
        var act = () => _service.Add("علی");

        act.Should().Throw<InvalidOperationException>();
    }
}
