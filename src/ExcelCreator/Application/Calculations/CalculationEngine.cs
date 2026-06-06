using ExcelCreator.Core.Abstractions;
using ExcelCreator.Localization;
using ExcelCreator.Core.Models;

namespace ExcelCreator.Application.Calculations;

public sealed class CalculationEngine : ICalculationEngine
{
    private readonly ICalculationActionRegistry _registry;

    public CalculationEngine(ICalculationActionRegistry registry)
    {
        _registry = registry;
    }

    public bool IsAvailable(CalculationActionDefinition action, CalculationContext context)
    {
        if (action.RequiresRows && context.Rows.Count == 0)
            return false;

        if (action.RequiresNumericColumns &&
            !context.Sheet.Columns.Any(CalculationValueParser.IsCalculableColumn))
        {
            return false;
        }

        if (action.MinimumNumericColumns > 0)
        {
            var calculableCount = context.Sheet.Columns.Count(CalculationValueParser.IsCalculableColumn);
            if (calculableCount < action.MinimumNumericColumns)
                return false;
        }

        if (action.MinimumColumnCount > 0 && context.Sheet.Columns.Count < action.MinimumColumnCount)
            return false;

        if (action.InputKind == CalculationInputKind.TwoTextColumns)
        {
            var textCount = context.Sheet.Columns.Count(column =>
                column.Type.Equals(ColumnTypes.Text, StringComparison.OrdinalIgnoreCase));
            if (textCount < 2)
                return false;
        }

        return true;
    }

    public CalculationResult Execute(string actionId, CalculationContext context, CalculationParameters? parameters = null)
    {
        var action = _registry.GetById(actionId);
        if (action is null)
            return CalculationResult.Failure(actionId, PersianStrings.CalculationUnknownAction);

        if (!IsAvailable(action, context))
            return CalculationResult.Failure(action.Label, PersianStrings.CalculationNotAvailable);

        if (action.InputKind != CalculationInputKind.None && parameters is null)
            return CalculationResult.Failure(action.Label, PersianStrings.CalculationMissingColumns);

        return actionId switch
        {
            CalculationActionIds.Sum => SumColumns(action, context),
            CalculationActionIds.Average => AverageColumns(action, context),
            CalculationActionIds.Min => MinColumns(action, context),
            CalculationActionIds.Max => MaxColumns(action, context),
            CalculationActionIds.CountRows => CountRows(action, context),
            CalculationActionIds.CountNumbers => CountNumbers(action, context),
            CalculationActionIds.RowTotals => RowTotals(action, context),
            CalculationActionIds.Percentage => Percentage(action, context, parameters!),
            CalculationActionIds.ColumnDifference => ColumnDifference(action, context, parameters!),
            CalculationActionIds.ColumnCompare => ColumnCompare(action, context, parameters!),
            CalculationActionIds.TimeDifference => TimeDifference(action, context, parameters!),
            _ => CalculationResult.Failure(action.Label, PersianStrings.CalculationUnknownAction)
        };
    }

    private static CalculationResult SumColumns(CalculationActionDefinition action, CalculationContext context)
    {
        var items = BuildNumericColumnItems(context, columnValues =>
        {
            var sum = columnValues.Sum();
            return (sum, CalculationFormatter.FormatNumber(sum));
        });

        if (items.Count == 0)
            return CalculationResult.Failure(action.Label, PersianStrings.CalculationNoNumericValues);

        return new CalculationResult
        {
            ActionLabel = action.Label,
            Items = items,
            Summary = PersianStrings.CalculationSumSummary
        };
    }

    private static CalculationResult AverageColumns(CalculationActionDefinition action, CalculationContext context)
    {
        var items = BuildNumericColumnItems(context, columnValues =>
        {
            if (columnValues.Count == 0)
                return (null, string.Empty);

            var average = columnValues.Average();
            return (average, CalculationFormatter.FormatNumber(average));
        }).Where(item => !string.IsNullOrEmpty(item.FormattedValue)).ToList();

        if (items.Count == 0)
            return CalculationResult.Failure(action.Label, PersianStrings.CalculationNoNumericValues);

        return new CalculationResult
        {
            ActionLabel = action.Label,
            Items = items,
            Summary = PersianStrings.CalculationAverageSummary
        };
    }

    private static CalculationResult MinColumns(CalculationActionDefinition action, CalculationContext context)
    {
        var items = BuildNumericColumnItems(context, columnValues =>
        {
            if (columnValues.Count == 0)
                return (null, string.Empty);

            var min = columnValues.Min();
            return (min, CalculationFormatter.FormatNumber(min));
        }).Where(item => !string.IsNullOrEmpty(item.FormattedValue)).ToList();

        if (items.Count == 0)
            return CalculationResult.Failure(action.Label, PersianStrings.CalculationNoNumericValues);

        return new CalculationResult
        {
            ActionLabel = action.Label,
            Items = items
        };
    }

    private static CalculationResult MaxColumns(CalculationActionDefinition action, CalculationContext context)
    {
        var items = BuildNumericColumnItems(context, columnValues =>
        {
            if (columnValues.Count == 0)
                return (null, string.Empty);

            var max = columnValues.Max();
            return (max, CalculationFormatter.FormatNumber(max));
        }).Where(item => !string.IsNullOrEmpty(item.FormattedValue)).ToList();

        if (items.Count == 0)
            return CalculationResult.Failure(action.Label, PersianStrings.CalculationNoNumericValues);

        return new CalculationResult
        {
            ActionLabel = action.Label,
            Items = items
        };
    }

    private static CalculationResult CountRows(CalculationActionDefinition action, CalculationContext context) =>
        new()
        {
            ActionLabel = action.Label,
            Items =
            [
                new CalculationResultItem
                {
                    Label = PersianStrings.CalculationCountRowsLabel,
                    FormattedValue = CalculationFormatter.FormatInteger(context.Rows.Count)
                }
            ]
        };

    private static CalculationResult CountNumbers(CalculationActionDefinition action, CalculationContext context)
    {
        var items = new List<CalculationResultItem>();
        var total = 0;

        for (var colIndex = 0; colIndex < context.Sheet.Columns.Count; colIndex++)
        {
            var column = context.Sheet.Columns[colIndex];
            if (!CalculationValueParser.IsCalculableColumn(column))
                continue;

            var count = 0;
            foreach (var row in context.Rows)
            {
                var value = colIndex < row.Values.Count ? row.Values[colIndex] : string.Empty;
                if (CalculationValueParser.TryParseDecimal(value, out _))
                    count++;
            }

            if (count == 0)
                continue;

            total += count;
            items.Add(new CalculationResultItem
            {
                Label = column.Header,
                FormattedValue = CalculationFormatter.FormatInteger(count)
            });
        }

        if (items.Count == 0)
            return CalculationResult.Failure(action.Label, PersianStrings.CalculationNoNumericValues);

        return new CalculationResult
        {
            ActionLabel = action.Label,
            Items = items,
            Summary = string.Format(PersianStrings.CalculationCountNumbersSummary, CalculationFormatter.FormatInteger(total))
        };
    }

    private static CalculationResult RowTotals(CalculationActionDefinition action, CalculationContext context)
    {
        var numericColumnIndexes = context.Sheet.Columns
            .Select((column, index) => (column, index))
            .Where(pair => CalculationValueParser.IsCalculableColumn(pair.column))
            .Select(pair => pair.index)
            .ToList();

        if (numericColumnIndexes.Count == 0)
            return CalculationResult.Failure(action.Label, PersianStrings.CalculationNotAvailable);

        var items = new List<CalculationResultItem>();
        decimal grandTotal = 0;

        for (var rowIndex = 0; rowIndex < context.Rows.Count; rowIndex++)
        {
            var row = context.Rows[rowIndex];
            decimal rowSum = 0;
            var hasValue = false;

            foreach (var colIndex in numericColumnIndexes)
            {
                var value = colIndex < row.Values.Count ? row.Values[colIndex] : string.Empty;
                if (CalculationValueParser.TryParseDecimal(value, out var parsed))
                {
                    rowSum += parsed;
                    hasValue = true;
                }
            }

            if (!hasValue)
                continue;

            grandTotal += rowSum;
            items.Add(new CalculationResultItem
            {
                Label = string.Format(PersianStrings.CalculationRowLabel, CalculationFormatter.FormatInteger(rowIndex + 1)),
                FormattedValue = CalculationFormatter.FormatNumber(rowSum)
            });
        }

        if (items.Count == 0)
            return CalculationResult.Failure(action.Label, PersianStrings.CalculationNoNumericValues);

        return new CalculationResult
        {
            ActionLabel = action.Label,
            Items = items,
            Summary = string.Format(PersianStrings.CalculationRowTotalsSummary, CalculationFormatter.FormatNumber(grandTotal))
        };
    }

    private static List<CalculationResultItem> BuildNumericColumnItems(
        CalculationContext context,
        Func<List<decimal>, (decimal? Value, string Formatted)> compute)
    {
        var items = new List<CalculationResultItem>();

        for (var colIndex = 0; colIndex < context.Sheet.Columns.Count; colIndex++)
        {
            var column = context.Sheet.Columns[colIndex];
            if (!CalculationValueParser.IsCalculableColumn(column))
                continue;

            var values = new List<decimal>();
            foreach (var row in context.Rows)
            {
                var raw = colIndex < row.Values.Count ? row.Values[colIndex] : string.Empty;
                if (CalculationValueParser.TryParseDecimal(raw, out var parsed))
                    values.Add(parsed);
            }

            if (values.Count == 0)
                continue;

            var (value, formatted) = compute(values);
            if (value is null || string.IsNullOrEmpty(formatted))
                continue;

            items.Add(new CalculationResultItem
            {
                Label = column.Header,
                FormattedValue = formatted
            });
        }

        return items;
    }

    private static CalculationResult Percentage(
        CalculationActionDefinition action,
        CalculationContext context,
        CalculationParameters parameters)
    {
        var colIndex = parameters.PrimaryColumnIndex;
        var column = context.Sheet.Columns[colIndex];
        var values = CollectNumericValues(context, colIndex);
        if (values.Count == 0)
            return CalculationResult.Failure(action.Label, PersianStrings.CalculationNoNumericValues);

        var total = values.Sum(pair => pair.Value);
        if (total == 0)
            return CalculationResult.Failure(action.Label, PersianStrings.CalculationNoNumericValues);

        var items = values
            .Select(pair => new CalculationResultItem
            {
                Label = string.Format(PersianStrings.CalculationRowLabel, CalculationFormatter.FormatInteger(pair.RowNumber)),
                FormattedValue = CalculationFormatter.FormatPercent(pair.Value / total * 100m)
            })
            .ToList();

        return new CalculationResult
        {
            ActionLabel = action.Label,
            Items = items,
            Summary = $"{PersianStrings.CalculationPercentageSummary} — {column.Header}"
        };
    }

    private static CalculationResult ColumnDifference(
        CalculationActionDefinition action,
        CalculationContext context,
        CalculationParameters parameters)
    {
        var items = BuildColumnPairItems(
            context,
            parameters,
            (left, right) => CalculationFormatter.FormatNumber(left - right));

        if (items.Count == 0)
            return CalculationResult.Failure(action.Label, PersianStrings.CalculationNoNumericValues);

        var leftHeader = context.Sheet.Columns[parameters.PrimaryColumnIndex].Header;
        var rightHeader = context.Sheet.Columns[parameters.SecondaryColumnIndex].Header;

        return new CalculationResult
        {
            ActionLabel = action.Label,
            Items = items,
            Summary = $"{PersianStrings.CalculationColumnDifferenceSummary}: {leftHeader} − {rightHeader}"
        };
    }

    private static CalculationResult ColumnCompare(
        CalculationActionDefinition action,
        CalculationContext context,
        CalculationParameters parameters)
    {
        var items = BuildColumnPairItems(
            context,
            parameters,
            (left, right) => left <= right
                ? PersianStrings.CalculationCompareLessOrEqual
                : PersianStrings.CalculationCompareGreater);

        if (items.Count == 0)
            return CalculationResult.Failure(action.Label, PersianStrings.CalculationNoNumericValues);

        var leftHeader = context.Sheet.Columns[parameters.PrimaryColumnIndex].Header;
        var rightHeader = context.Sheet.Columns[parameters.SecondaryColumnIndex].Header;

        return new CalculationResult
        {
            ActionLabel = action.Label,
            Items = items,
            Summary = $"{leftHeader} ↔ {rightHeader}"
        };
    }

    private static CalculationResult TimeDifference(
        CalculationActionDefinition action,
        CalculationContext context,
        CalculationParameters parameters)
    {
        var items = new List<CalculationResultItem>();

        for (var rowIndex = 0; rowIndex < context.Rows.Count; rowIndex++)
        {
            var row = context.Rows[rowIndex];
            var startRaw = GetCellValue(row, parameters.PrimaryColumnIndex);
            var endRaw = GetCellValue(row, parameters.SecondaryColumnIndex);

            if (!TimeValueParser.TryParseTime(startRaw, out var start) ||
                !TimeValueParser.TryParseTime(endRaw, out var end))
            {
                continue;
            }

            var duration = TimeValueParser.CalculateDuration(start, end);
            items.Add(new CalculationResultItem
            {
                Label = string.Format(PersianStrings.CalculationRowLabel, CalculationFormatter.FormatInteger(rowIndex + 1)),
                FormattedValue = TimeValueParser.FormatDuration(duration)
            });
        }

        if (items.Count == 0)
            return CalculationResult.Failure(action.Label, PersianStrings.CalculationNoTimeValues);

        var startHeader = context.Sheet.Columns[parameters.PrimaryColumnIndex].Header;
        var endHeader = context.Sheet.Columns[parameters.SecondaryColumnIndex].Header;

        return new CalculationResult
        {
            ActionLabel = action.Label,
            Items = items,
            Summary = $"{PersianStrings.CalculationTimeDifferenceSummary}: {endHeader} − {startHeader}"
        };
    }

    private static List<CalculationResultItem> BuildColumnPairItems(
        CalculationContext context,
        CalculationParameters parameters,
        Func<decimal, decimal, string> compute)
    {
        var items = new List<CalculationResultItem>();

        for (var rowIndex = 0; rowIndex < context.Rows.Count; rowIndex++)
        {
            var row = context.Rows[rowIndex];
            var leftRaw = GetCellValue(row, parameters.PrimaryColumnIndex);
            var rightRaw = GetCellValue(row, parameters.SecondaryColumnIndex);

            if (!CalculationValueParser.TryParseDecimal(leftRaw, out var left) ||
                !CalculationValueParser.TryParseDecimal(rightRaw, out var right))
            {
                continue;
            }

            items.Add(new CalculationResultItem
            {
                Label = string.Format(PersianStrings.CalculationRowLabel, CalculationFormatter.FormatInteger(rowIndex + 1)),
                FormattedValue = compute(left, right)
            });
        }

        return items;
    }

    private static List<(int RowNumber, decimal Value)> CollectNumericValues(CalculationContext context, int colIndex)
    {
        var values = new List<(int RowNumber, decimal Value)>();

        for (var rowIndex = 0; rowIndex < context.Rows.Count; rowIndex++)
        {
            var raw = GetCellValue(context.Rows[rowIndex], colIndex);
            if (CalculationValueParser.TryParseDecimal(raw, out var parsed))
                values.Add((rowIndex + 1, parsed));
        }

        return values;
    }

    private static string GetCellValue(TableRow row, int colIndex) =>
        colIndex < row.Values.Count ? row.Values[colIndex] : string.Empty;
}
