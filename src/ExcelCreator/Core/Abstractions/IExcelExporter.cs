using ExcelCreator.Core.Models;

namespace ExcelCreator.Core.Abstractions;

public interface IExcelExporter
{
    void Export(GenerationRequest request, string outputPath);
}
