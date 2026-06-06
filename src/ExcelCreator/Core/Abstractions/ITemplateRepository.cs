using ExcelCreator.Core.Models;

namespace ExcelCreator.Core.Abstractions;

public interface ITemplateRepository
{
    IReadOnlyList<TemplateDefinition> LoadAll();
    TemplateDefinition? GetById(string id);
    void InvalidateCache();
}
