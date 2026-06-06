using ExcelCreator.Application.Calculations;

namespace ExcelCreator.Core.Abstractions;

public interface ICalculationEngine
{
    CalculationResult Execute(string actionId, CalculationContext context, CalculationParameters? parameters = null);

    bool IsAvailable(CalculationActionDefinition action, CalculationContext context);
}
