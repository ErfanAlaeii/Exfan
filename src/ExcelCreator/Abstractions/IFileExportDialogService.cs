using System.Windows;

namespace ExcelCreator.Abstractions;

public interface IFileExportDialogService
{
    bool TryGetSavePath(Window owner, string suggestedFileName, string? initialDirectory, out string filePath);
    void NotifyExportSuccess(string filePath, bool openAfterCreate);
    void NotifyError(string message);
    void NotifyValidationError(string message);
    void NotifyInfo(string message);
    void NotifyWarning(string message);
    MessageBoxResult Confirm(string message, MessageBoxButton buttons, MessageBoxImage icon);
}
