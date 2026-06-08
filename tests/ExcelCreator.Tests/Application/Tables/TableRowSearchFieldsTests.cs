using ExcelCreator.Application.Tables;
using ExcelCreator.Core.Models;
using ExcelCreator.Localization;
using FluentAssertions;

namespace ExcelCreator.Tests.Application.Tables;

public class TableRowSearchFieldsTests
{
    [Fact]
    public void FromSheet_IncludesTemplateColumnsAndCreatedAt()
    {
        var sheet = new SheetSpec
        {
            Columns =
            [
                new ColumnSpec { Header = "نام", Type = ColumnTypes.Text },
                new ColumnSpec { Header = "تاریخ", Type = ColumnTypes.Date }
            ]
        };

        var fields = TableRowSearchFields.FromSheet(sheet);

        fields.Should().HaveCount(3);
        fields[0].Header.Should().Be("نام");
        fields[0].InputKind.Should().Be(TableRowSearchInputKind.Text);
        fields[1].Header.Should().Be("تاریخ");
        fields[1].InputKind.Should().Be(TableRowSearchInputKind.Date);
        fields[2].Target.Should().Be(TableRowSearchTarget.CreatedAt);
        fields[2].Header.Should().Be(PersianStrings.RowTimestampColumnHeader);
        fields[2].InputKind.Should().Be(TableRowSearchInputKind.Date);
    }
}
