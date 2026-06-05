namespace ExcelCreator.Tests.Helpers;

public sealed class TestAppLogger : ExcelCreator.Abstractions.IAppLogger
{
    public List<string> Messages { get; } = [];

    public void Info(string message) => Messages.Add($"INFO: {message}");

    public void Warning(string message) => Messages.Add($"WARN: {message}");

    public void Error(string message, Exception? exception = null) =>
        Messages.Add($"ERROR: {message}");
}
