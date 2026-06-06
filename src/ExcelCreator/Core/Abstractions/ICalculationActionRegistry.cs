using ExcelCreator.Application.Calculations;

namespace ExcelCreator.Core.Abstractions;

public interface ICalculationActionRegistry
{
    IReadOnlyList<CalculationActionDefinition> GetAll();

    CalculationActionDefinition? GetById(string id);
}
