using ExcelCreator.Application.Calculations;
using ExcelCreator.Application.Export;
using ExcelCreator.Application.Images;
using ExcelCreator.Application.Settings;
using ExcelCreator.Application.Tables;
using ExcelCreator.Application.Templates;
using ExcelCreator.Core.Abstractions;
using ExcelCreator.Infrastructure.Logging;
using ExcelCreator.UI.Navigation;
using ExcelCreator.UI.Presentation;
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
        services.AddSingleton<IImagePickerService, WpfImagePickerService>();
        services.AddSingleton<IImageStorageService, ImageStorageService>();
        services.AddSingleton<IExcelExportFacade, ExcelExportFacade>();
        services.AddSingleton<ICalculationActionRegistry, CalculationActionRegistry>();
        services.AddSingleton<ICalculationEngine, CalculationEngine>();
        services.AddSingleton<IAppNavigator, AppNavigator>();

        _provider = services.BuildServiceProvider();
    }

    public static T GetRequiredService<T>() where T : notnull =>
        Provider.GetRequiredService<T>();
}
