using ExcelCreator.Models;

namespace ExcelCreator.Abstractions;

public interface IExcelExporter
{
    void Export(GenerationRequest request, string outputPath);
}
