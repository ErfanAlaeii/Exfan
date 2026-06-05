using ExcelCreator.Abstractions;
using ExcelCreator.Infrastructure;
using ExcelCreator.Navigation;
using ExcelCreator.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ExcelCreator.Composition;

public static class ServiceRegistration
{
    private static IServiceProvider? _provider;

    public static IServiceProvider Provider =>
        _provider ?? throw new InvalidOperationException("Services have not been configured.");

    public static void Configure()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IAppLogger, FileAppLogger>();
        services.AddSingleton<IUserSettingsStore, PresetService>();
        services.AddSingleton<ISavedTableRepository, SavedTableService>();
        services.AddSingleton<ITemplateRepository, TemplateService>();
        services.AddSingleton<IExcelExporter, ExcelWorkbookBuilder>();
        services.AddSingleton<IFileExportDialogService, WpfFileExportDialogService>();
        services.AddSingleton<IExcelExportFacade, ExcelExportFacade>();
        services.AddSingleton<ICalculationActionRegistry, CalculationActionRegistry>();
        services.AddSingleton<ICalculationEngine, CalculationEngine>();
        services.AddSingleton<IAppNavigator, AppNavigator>();

        _provider = services.BuildServiceProvider();
    }

    public static T GetRequiredService<T>() where T : notnull =>
        Provider.GetRequiredService<T>();
}
