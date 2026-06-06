using System.IO;
using ExcelCreator.Core.Abstractions;
using ExcelCreator.Infrastructure.Paths;

namespace ExcelCreator.Infrastructure.Logging;

public sealed class FileAppLogger : IAppLogger
{
    private readonly string _logPath;
    private readonly object _lock = new();

    public FileAppLogger()
    {
        _logPath = Path.Combine(AppPaths.AppDataDirectory, "app.log");
    }

    public void Info(string message) => Write("INFO", message);

    public void Warning(string message) => Write("WARN", message);

    public void Error(string message, Exception? exception = null)
    {
        var detail = exception is null ? message : $"{message}{Environment.NewLine}{exception}";
        Write("ERROR", detail);
    }

    private void Write(string level, string message)
    {
        var line = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} [{level}] {message}";
        lock (_lock)
        {
            try
            {
                File.AppendAllText(_logPath, line + Environment.NewLine);
            }
            catch
            {
                // Logging must not crash the app.
            }
        }
    }
}
