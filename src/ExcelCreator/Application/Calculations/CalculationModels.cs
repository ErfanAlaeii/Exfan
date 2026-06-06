using ExcelCreator.Core.Models;

namespace ExcelCreator.Application.Calculations;

public enum CalculationScope
{
    ColumnAggregate,
    TableInfo,
    PerRow
}

public enum CalculationInputKind
{
    None,
    SingleNumericColumn,
    TwoNumericColumns,
    TwoTextColumns
}

public sealed class CalculationActionDefinition
{
    public required string Id { get; init; }
    public required string Label { get; init; }
    public required string Description { get; init; }
    public CalculationScope Scope { get; init; }
    public CalculationInputKind InputKind { get; init; } = CalculationInputKind.None;
    public bool RequiresNumericColumns { get; init; }
    public bool RequiresRows { get; init; } = true;
    public int MinimumNumericColumns { get; init; }
    public int MinimumColumnCount { get; init; }
}

public sealed class CalculationParameters
{
    public int PrimaryColumnIndex { get; init; }
    public int SecondaryColumnIndex { get; init; }
}

public sealed class CalculationContext
{
    public required SheetSpec Sheet { get; init; }
    public required IReadOnlyList<TableRow> Rows { get; init; }
    public DateCalendarKind Calendar { get; init; } = DateCalendarKind.Jalali;
}

public sealed class CalculationResultItem
{
    public required string Label { get; init; }
    public required string FormattedValue { get; init; }
}

public sealed class CalculationResult
{
    public required string ActionLabel { get; init; }
    public required IReadOnlyList<CalculationResultItem> Items { get; init; }
    public string? Summary { get; init; }
    public bool IsSuccess { get; init; } = true;
    public string? ErrorMessage { get; init; }

    public static CalculationResult Failure(string actionLabel, string message) => new()
    {
        ActionLabel = actionLabel,
        Items = [],
        IsSuccess = false,
        ErrorMessage = message
    };
}
