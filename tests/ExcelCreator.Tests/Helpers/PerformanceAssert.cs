using System.Diagnostics;

namespace ExcelCreator.Tests.Helpers;

internal static class PerformanceAssert
{
    public static TimeSpan Measure(Action action)
    {
        action();
        var sw = Stopwatch.StartNew();
        action();
        sw.Stop();
        return sw.Elapsed;
    }

    public static void CompletesWithin(Action action, TimeSpan maxDuration, string operationName)
    {
        var sw = Stopwatch.StartNew();
        action();
        sw.Stop();

        if (sw.Elapsed > maxDuration)
        {
            throw new Xunit.Sdk.XunitException(
                $"{operationName} took {sw.Elapsed.TotalMilliseconds:F0} ms; limit is {maxDuration.TotalMilliseconds:F0} ms.");
        }
    }
}
