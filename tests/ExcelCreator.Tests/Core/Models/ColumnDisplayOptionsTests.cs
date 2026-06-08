using ExcelCreator.Core.Models;
using FluentAssertions;

namespace ExcelCreator.Tests.Core.Models;

public class ColumnDisplayOptionsTests
{
    [Fact]
    public void IsMultiline_ReturnsTrueWhenFlagSet()
    {
        var column = new ColumnSpec { Header = "شرح", Multiline = true };
        ColumnDisplayOptions.IsMultiline(column).Should().BeTrue();
    }

    [Fact]
    public void SheetHasMultilineColumns_DetectsAnyMultilineColumn()
    {
        var sheet = new SheetSpec
        {
            Columns =
            [
                new ColumnSpec { Header = "نام", Type = ColumnTypes.Text },
                new ColumnSpec { Header = "شرح", Type = ColumnTypes.Text, Multiline = true }
            ]
        };

        ColumnDisplayOptions.SheetHasMultilineColumns(sheet).Should().BeTrue();
    }
}
