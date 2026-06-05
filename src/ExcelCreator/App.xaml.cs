using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using ExcelCreator.Composition;
using ExcelCreator.Infrastructure;

namespace ExcelCreator;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        RegisterExceptionHandlers();

        var culture = new CultureInfo("fa-IR");
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;

        try
        {
            ServiceRegistration.Configure();
        }
        catch (Exception ex)
        {
            WriteCrashLog(ex);
            MessageBox.Show(
                $"خطا در راه‌اندازی برنامه:{Environment.NewLine}{Environment.NewLine}{ex.Message}",
                "Exfan",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown(1);
            return;
        }

        base.OnStartup(e);
    }

    private void RegisterExceptionHandlers()
    {
        DispatcherUnhandledException += (_, args) =>
        {
            WriteCrashLog(args.Exception);
            MessageBox.Show(
                $"خطای غیرمنتظره:{Environment.NewLine}{Environment.NewLine}{args.Exception.Message}",
                "Exfan",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            args.Handled = true;
        };

        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            if (args.ExceptionObject is Exception ex)
                WriteCrashLog(ex);
        };

        TaskScheduler.UnobservedTaskException += (_, args) =>
        {
            WriteCrashLog(args.Exception);
            args.SetObserved();
        };
    }

    private static void WriteCrashLog(Exception ex)
    {
        try
        {
            var path = Path.Combine(AppPaths.AppDataDirectory, "crash.log");
            File.AppendAllText(path, $"{DateTime.UtcNow:u}{Environment.NewLine}{ex}{Environment.NewLine}{Environment.NewLine}");
        }
        catch
        {
            // Best effort only.
        }
    }
}
