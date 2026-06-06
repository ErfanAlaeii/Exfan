using ExcelCreator.Core.Models;

namespace ExcelCreator.Core.Abstractions;

public interface ISavedTableRepository
{
    IReadOnlyList<SavedTable> GetByTemplate(string templateId);
    SavedTable? GetById(string id);
    SavedTable Save(SavedTable table, TemplateDefinition template);
    bool Delete(string id);
    bool NameExistsForTemplate(string templateId, string name, string? excludeId = null);
}
