using ExcelCreator.Models;

namespace ExcelCreator.Abstractions;

public interface ICalculationActionRegistry
{
    IReadOnlyList<CalculationActionDefinition> GetAll();

    CalculationActionDefinition? GetById(string id);
}
