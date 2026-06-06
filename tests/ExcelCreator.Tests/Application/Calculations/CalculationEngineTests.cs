using ExcelCreator.Localization;
using ExcelCreator.Core.Models;
using ExcelCreator.Application.Calculations;
using ExcelCreator.Application.Common;
using FluentAssertions;

namespace ExcelCreator.Tests.Application.Calculations;

public class CalculationEngineTests
{
    private static readonly SheetSpec Sheet = new()
    {
        Columns =
        [
            new ColumnSpec { Header = "نام", Type = ColumnTypes.Text },
            new ColumnSpec { Header = "مبلغ", Type = ColumnTypes.Currency },
            new ColumnSpec { Header = "تعداد", Type = ColumnTypes.Number }
        ]
    };

    private static readonly IReadOnlyList<TableRow> Rows =
    [
        TableRow.CreateNew(["علی", "1000", "2"]),
        TableRow.CreateNew(["رضا", "500", "3"])
    ];

    private readonly CalculationEngine _engine = new(new CalculationActionRegistry());

    private CalculationContext CreateContext() => new()
    {
        Sheet = Sheet,
        Rows = Rows,
        Calendar = DateCalendarKind.Jalali
    };

    [Fact]
    public void Sum_ComputesColumnTotals()
    {
        var result = _engine.Execute(CalculationActionIds.Sum, CreateContext());

        result.IsSuccess.Should().BeTrue();
        result.Items.Should().HaveCount(2);
        result.Items[0].Label.Should().Be("مبلغ");
        result.Items[0].FormattedValue.Should().Be("۱,۵۰۰");
        result.Items[1].Label.Should().Be("تعداد");
        result.Items[1].FormattedValue.Should().Be("۵");
    }

    [Fact]
    public void Average_ComputesColumnAverages()
    {
        var result = _engine.Execute(CalculationActionIds.Average, CreateContext());

        result.IsSuccess.Should().BeTrue();
        result.Items[0].FormattedValue.Should().Be("۷۵۰");
        result.Items[1].FormattedValue.Should().Be("۲.۵");
    }

    [Fact]
    public void CountRows_ReturnsRowCount()
    {
        var result = _engine.Execute(CalculationActionIds.CountRows, CreateContext());

        result.IsSuccess.Should().BeTrue();
        result.Items[0].FormattedValue.Should().Be("۲");
    }

    [Fact]
    public void RowTotals_ComputesPerRowAndGrandTotal()
    {
        var result = _engine.Execute(CalculationActionIds.RowTotals, CreateContext());

        result.IsSuccess.Should().BeTrue();
        result.Items.Should().HaveCount(2);
        result.Summary.Should().Contain("۱,۵۰۵");
    }

    [Fact]
    public void IsAvailable_AllowsSumOnTextColumnsThatHoldNumbers()
    {
        var sheet = new SheetSpec
        {
            Columns =
            [
                new ColumnSpec { Header = "ساعت ورود", Type = ColumnTypes.Text },
                new ColumnSpec { Header = "ساعت خروج", Type = ColumnTypes.Text }
            ]
        };
        var context = new CalculationContext
        {
            Sheet = sheet,
            Rows = [TableRow.CreateNew(["11", "18"])]
        };
        var sumAction = new CalculationActionRegistry().GetById(CalculationActionIds.Sum)!;

        _engine.IsAvailable(sumAction, context).Should().BeTrue();
    }

    [Fact]
    public void Sum_WorksOnPersonnelStyleTimeColumns()
    {
        var sheet = new SheetSpec
        {
            Columns =
            [
                new ColumnSpec { Header = "نام", Type = ColumnTypes.Text },
                new ColumnSpec { Header = "ساعت ورود", Type = ColumnTypes.Text },
                new ColumnSpec { Header = "ساعت خروج", Type = ColumnTypes.Text }
            ]
        };
        var context = new CalculationContext
        {
            Sheet = sheet,
            Rows =
            [
                TableRow.CreateNew(["عرفان", "11", "18"]),
                TableRow.CreateNew(["علی", "5", "2"])
            ]
        };

        var result = _engine.Execute(CalculationActionIds.Sum, context);

        result.IsSuccess.Should().BeTrue();
        result.Items.Should().HaveCount(2);
        result.Items[0].Label.Should().Be("ساعت ورود");
        result.Items[0].FormattedValue.Should().Be("۱۶");
        result.Items[1].Label.Should().Be("ساعت خروج");
        result.Items[1].FormattedValue.Should().Be("۲۰");
    }

    [Fact]
    public void Percentage_ComputesRowSharesOfColumnTotal()
    {
        var parameters = new CalculationParameters { PrimaryColumnIndex = 1, SecondaryColumnIndex = 1 };
        var result = _engine.Execute(CalculationActionIds.Percentage, CreateContext(), parameters);

        result.IsSuccess.Should().BeTrue();
        result.Items[0].FormattedValue.Should().Be("۶۶.۶۷%");
        result.Items[1].FormattedValue.Should().Be("۳۳.۳۳%");
    }

    [Fact]
    public void ColumnDifference_SubtractsSecondColumnFromFirst()
    {
        var parameters = new CalculationParameters { PrimaryColumnIndex = 1, SecondaryColumnIndex = 2 };
        var result = _engine.Execute(CalculationActionIds.ColumnDifference, CreateContext(), parameters);

        result.IsSuccess.Should().BeTrue();
        result.Items[0].FormattedValue.Should().Be("۹۹۸");
        result.Items[1].FormattedValue.Should().Be("۴۹۷");
    }

    [Fact]
    public void ColumnCompare_ClassifiesRowsAsLessOrEnough()
    {
        var parameters = new CalculationParameters { PrimaryColumnIndex = 2, SecondaryColumnIndex = 1 };
        var result = _engine.Execute(CalculationActionIds.ColumnCompare, CreateContext(), parameters);

        result.IsSuccess.Should().BeTrue();
        result.Items[0].FormattedValue.Should().Be(PersianStrings.CalculationCompareLessOrEqual);
        result.Items[1].FormattedValue.Should().Be(PersianStrings.CalculationCompareLessOrEqual);
    }

    [Fact]
    public void TimeDifference_ComputesDurationBetweenTimeColumns()
    {
        var sheet = new SheetSpec
        {
            Columns =
            [
                new ColumnSpec { Header = "نام", Type = ColumnTypes.Text },
                new ColumnSpec { Header = "ساعت ورود", Type = ColumnTypes.Text },
                new ColumnSpec { Header = "ساعت خروج", Type = ColumnTypes.Text }
            ]
        };
        var context = new CalculationContext
        {
            Sheet = sheet,
            Rows =
            [
                TableRow.CreateNew(["علی", "08:00", "17:00"]),
                TableRow.CreateNew(["رضا", "09:30", "18:15"])
            ]
        };
        var parameters = new CalculationParameters { PrimaryColumnIndex = 1, SecondaryColumnIndex = 2 };
        var result = _engine.Execute(CalculationActionIds.TimeDifference, context, parameters);

        result.IsSuccess.Should().BeTrue();
        result.Items[0].FormattedValue.Should().Be("۹:۰۰");
        result.Items[1].FormattedValue.Should().Be("۸:۴۵");
    }

    [Fact]
    public void TimeDifference_AcceptsBareHourValues()
    {
        var sheet = new SheetSpec
        {
            Columns =
            [
                new ColumnSpec { Header = "نام", Type = ColumnTypes.Text },
                new ColumnSpec { Header = "ساعت ورود", Type = ColumnTypes.Text },
                new ColumnSpec { Header = "ساعت خروج", Type = ColumnTypes.Text }
            ]
        };
        var context = new CalculationContext
        {
            Sheet = sheet,
            Rows = [TableRow.CreateNew(["عرفان", "11", "18"])]
        };
        var parameters = new CalculationParameters { PrimaryColumnIndex = 1, SecondaryColumnIndex = 2 };
        var result = _engine.Execute(CalculationActionIds.TimeDifference, context, parameters);

        result.IsSuccess.Should().BeTrue();
        result.Items[0].FormattedValue.Should().Be("۷:۰۰");
    }
}
