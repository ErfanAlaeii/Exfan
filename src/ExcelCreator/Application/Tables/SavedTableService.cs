using ExcelCreator.Core.Abstractions;
using ExcelCreator.Core.Models;
using ExcelCreator.Core.Validation;
using ExcelCreator.Infrastructure.Persistence;
using ExcelCreator.Infrastructure.Paths;

namespace ExcelCreator.Application.Tables;

public sealed class SavedTableService : ISavedTableRepository
{
    private readonly AtomicJsonStore<SavedTableStore> _store;
    private SavedTableStore? _cache;

    public SavedTableService(string? storeFilePath = null)
    {
        var path = storeFilePath ?? AppPaths.TablesFile;
        _store = new AtomicJsonStore<SavedTableStore>(path, JsonDefaults.Storage, "جداول ذخیره‌شده");
    }

    public IReadOnlyList<SavedTable> GetByTemplate(string templateId) =>
        LoadStore()
            .Tables
            .Where(t => t.TemplateId.Equals(templateId, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(t => t.UpdatedAt)
            .ToList();

    public SavedTable? GetById(string id) =>
        LoadStore().Tables.FirstOrDefault(t => t.Id.Equals(id, StringComparison.OrdinalIgnoreCase));

    public SavedTable Save(SavedTable table, TemplateDefinition template)
    {
        var templateValidation = TemplateValidator.Validate(template);
        if (!templateValidation.IsValid)
            throw new InvalidOperationException(templateValidation.Message);

        var compatibility = TableValidator.ValidateTemplateCompatibility(table, template);
        if (!compatibility.IsValid)
            throw new InvalidOperationException(compatibility.Message);

        var sheet = TableSchemaResolver.ResolveSheet(template, table);
        var validation = TableValidator.ValidateRows(sheet, table.Rows, table.DateCalendar);
        if (!validation.IsValid)
            throw new InvalidOperationException(validation.Message);

        table.Rows = TableValidator.NormalizeRows(sheet, table.Rows);
        table.TemplateId = template.Id;
        table.TemplateVersion = template.Version;
        table.SheetName = sheet.Name;
        table.ColumnHeaders = sheet.Columns.Select(c => c.Header).ToList();
        if (table.CustomColumns is { Count: > 0 })
            table.CustomColumns = TableSchemaResolver.CloneColumns(sheet.Columns);

        var store = LoadStore();
        var existing = store.Tables.FindIndex(t => t.Id.Equals(table.Id, StringComparison.OrdinalIgnoreCase));
        table.UpdatedAt = DateTime.UtcNow;

        if (existing >= 0)
            store.Tables[existing] = table;
        else
        {
            table.CreatedAt = DateTime.UtcNow;
            store.Tables.Add(table);
        }

        WriteStore(store);
        return table;
    }

    public bool Delete(string id)
    {
        var store = LoadStore();
        var removed = store.Tables.RemoveAll(t => t.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        if (removed == 0)
            return false;

        WriteStore(store);
        return true;
    }

    public bool NameExistsForTemplate(string templateId, string name, string? excludeId = null) =>
        LoadStore().Tables.Any(t =>
            t.TemplateId.Equals(templateId, StringComparison.OrdinalIgnoreCase) &&
            t.Name.Equals(name.Trim(), StringComparison.OrdinalIgnoreCase) &&
            (excludeId is null || !t.Id.Equals(excludeId, StringComparison.OrdinalIgnoreCase)));

    private SavedTableStore LoadStore()
    {
        if (_cache is not null)
            return _cache;

        _cache = _store.LoadOrDefault(() => new SavedTableStore());
        return _cache;
    }

    private void WriteStore(SavedTableStore store)
    {
        _store.Save(store);
        _cache = store;
    }
}
