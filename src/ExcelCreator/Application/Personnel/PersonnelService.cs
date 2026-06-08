using ExcelCreator.Core.Abstractions;
using ExcelCreator.Core.Models;
using ExcelCreator.Infrastructure.Persistence;
using ExcelCreator.Infrastructure.Paths;
using ExcelCreator.Localization;

namespace ExcelCreator.Application.Personnel;

public sealed class PersonnelService : IPersonnelRepository
{
    private readonly AtomicJsonStore<PersonnelStore> _store;
    private PersonnelStore? _cache;

    public PersonnelService(string? storeFilePath = null)
    {
        var path = storeFilePath ?? AppPaths.PersonnelFile;
        _store = new AtomicJsonStore<PersonnelStore>(path, JsonDefaults.Storage, "پرسنل");
    }

    public IReadOnlyList<PersonnelMember> GetAll() =>
        LoadStore()
            .Members
            .OrderBy(member => member.Name, StringComparer.CurrentCulture)
            .ToList();

    public IReadOnlyList<string> GetNames() =>
        GetAll().Select(member => member.Name).ToList();

    public PersonnelMember Add(string name)
    {
        var trimmed = NormalizeName(name);
        ValidateName(trimmed, excludeId: null);

        var member = new PersonnelMember
        {
            Name = trimmed,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var store = LoadStore();
        store.Members.Add(member);
        WriteStore(store);
        return member;
    }

    public PersonnelMember Update(string id, string name)
    {
        var trimmed = NormalizeName(name);
        ValidateName(trimmed, excludeId: id);

        var store = LoadStore();
        var index = store.Members.FindIndex(member => member.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        if (index < 0)
            throw new InvalidOperationException(PersianStrings.PersonnelNotFound);

        store.Members[index].Name = trimmed;
        store.Members[index].UpdatedAt = DateTime.UtcNow;
        WriteStore(store);
        return store.Members[index];
    }

    public bool Delete(string id)
    {
        var store = LoadStore();
        var removed = store.Members.RemoveAll(member => member.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        if (removed == 0)
            return false;

        WriteStore(store);
        return true;
    }

    public bool NameExists(string name, string? excludeId = null)
    {
        var trimmed = NormalizeName(name);
        return LoadStore().Members.Any(member =>
            member.Name.Equals(trimmed, StringComparison.CurrentCultureIgnoreCase) &&
            (excludeId is null || !member.Id.Equals(excludeId, StringComparison.OrdinalIgnoreCase)));
    }

    private static string NormalizeName(string name) => name.Trim();

    private void ValidateName(string name, string? excludeId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidOperationException(PersianStrings.PersonnelNameRequired);

        if (NameExists(name, excludeId))
            throw new InvalidOperationException(PersianStrings.PersonnelNameDuplicate);
    }

    private PersonnelStore LoadStore()
    {
        if (_cache is not null)
            return _cache;

        _cache = _store.LoadOrDefault(() => new PersonnelStore());
        return _cache;
    }

    private void WriteStore(PersonnelStore store)
    {
        _store.Save(store);
        _cache = store;
    }
}
