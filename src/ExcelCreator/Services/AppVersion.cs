using System.Reflection;

namespace ExcelCreator.Services;

public static class AppVersion
{
    public static string Informational =>
        Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
        ?? "1.0.0";

    public static string File =>
        Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version
        ?? "1.0.0.0";

    public static string Product =>
        Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyProductAttribute>()?.Product
        ?? "سازنده اکسل خالق: عرفان";
}
