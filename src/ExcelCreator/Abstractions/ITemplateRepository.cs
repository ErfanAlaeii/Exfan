using ExcelCreator.Models;

namespace ExcelCreator.Abstractions;

public interface ITemplateRepository
{
    IReadOnlyList<TemplateDefinition> LoadAll();
    TemplateDefinition? GetById(string id);
    void InvalidateCache();
}
