using ExcelCreator.Core.Models;

namespace ExcelCreator.Core.Abstractions;

public interface IPersonnelRepository
{
    IReadOnlyList<PersonnelMember> GetAll();

    IReadOnlyList<string> GetNames();

    PersonnelMember Add(string name);

    PersonnelMember Update(string id, string name);

    bool Delete(string id);

    bool NameExists(string name, string? excludeId = null);
}
