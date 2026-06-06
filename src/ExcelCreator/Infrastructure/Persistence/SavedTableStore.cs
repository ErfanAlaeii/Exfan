using ExcelCreator.Core.Models;

namespace ExcelCreator.Infrastructure.Persistence;

public sealed class SavedTableStore
{
    public List<SavedTable> Tables { get; set; } = [];
}
