using ExcelCreator.Abstractions;
using ExcelCreator.Localization;
using ExcelCreator.Models;

namespace ExcelCreator.Services;

public sealed class CalculationActionRegistry : ICalculationActionRegistry
{
    private static readonly IReadOnlyList<CalculationActionDefinition> Actions =
    [
        new()
        {
            Id = CalculationActionIds.Sum,
            Label = PersianStrings.CalculationSum,
            Description = PersianStrings.CalculationSumDescription,
            Scope = CalculationScope.ColumnAggregate,
            RequiresNumericColumns = true
        },
        new()
        {
            Id = CalculationActionIds.Average,
            Label = PersianStrings.CalculationAverage,
            Description = PersianStrings.CalculationAverageDescription,
            Scope = CalculationScope.ColumnAggregate,
            RequiresNumericColumns = true
        },
        new()
        {
            Id = CalculationActionIds.Min,
            Label = PersianStrings.CalculationMin,
            Description = PersianStrings.CalculationMinDescription,
            Scope = CalculationScope.ColumnAggregate,
            RequiresNumericColumns = true
        },
        new()
        {
            Id = CalculationActionIds.Max,
            Label = PersianStrings.CalculationMax,
            Description = PersianStrings.CalculationMaxDescription,
            Scope = CalculationScope.ColumnAggregate,
            RequiresNumericColumns = true
        },
        new()
        {
            Id = CalculationActionIds.CountRows,
            Label = PersianStrings.CalculationCountRows,
            Description = PersianStrings.CalculationCountRowsDescription,
            Scope = CalculationScope.TableInfo,
            RequiresNumericColumns = false
        },
        new()
        {
            Id = CalculationActionIds.CountNumbers,
            Label = PersianStrings.CalculationCountNumbers,
            Description = PersianStrings.CalculationCountNumbersDescription,
            Scope = CalculationScope.ColumnAggregate,
            RequiresNumericColumns = true
        },
        new()
        {
            Id = CalculationActionIds.RowTotals,
            Label = PersianStrings.CalculationRowTotals,
            Description = PersianStrings.CalculationRowTotalsDescription,
            Scope = CalculationScope.PerRow,
            RequiresNumericColumns = true
        },
        new()
        {
            Id = CalculationActionIds.Percentage,
            Label = PersianStrings.CalculationPercentage,
            Description = PersianStrings.CalculationPercentageDescription,
            Scope = CalculationScope.PerRow,
            InputKind = CalculationInputKind.SingleNumericColumn,
            RequiresNumericColumns = true,
            MinimumNumericColumns = 1
        },
        new()
        {
            Id = CalculationActionIds.ColumnDifference,
            Label = PersianStrings.CalculationColumnDifference,
            Description = PersianStrings.CalculationColumnDifferenceDescription,
            Scope = CalculationScope.PerRow,
            InputKind = CalculationInputKind.TwoNumericColumns,
            RequiresNumericColumns = true,
            MinimumNumericColumns = 2
        },
        new()
        {
            Id = CalculationActionIds.ColumnCompare,
            Label = PersianStrings.CalculationColumnCompare,
            Description = PersianStrings.CalculationColumnCompareDescription,
            Scope = CalculationScope.PerRow,
            InputKind = CalculationInputKind.TwoNumericColumns,
            RequiresNumericColumns = true,
            MinimumNumericColumns = 2
        },
        new()
        {
            Id = CalculationActionIds.TimeDifference,
            Label = PersianStrings.CalculationTimeDifference,
            Description = PersianStrings.CalculationTimeDifferenceDescription,
            Scope = CalculationScope.PerRow,
            InputKind = CalculationInputKind.TwoTextColumns,
            MinimumColumnCount = 2
        }
    ];

    public IReadOnlyList<CalculationActionDefinition> GetAll() => Actions;

    public CalculationActionDefinition? GetById(string id) =>
        Actions.FirstOrDefault(action => action.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
}
