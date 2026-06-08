using ExcelCreator.Core.Models;

namespace ExcelCreator.Core.Abstractions;

public interface IPrintService
{
    bool Print(TablePrintModel model, object? ownerWindow = null);
}
