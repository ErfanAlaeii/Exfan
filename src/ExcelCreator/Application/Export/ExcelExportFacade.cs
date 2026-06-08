using System.IO;
using System.Windows;
using ExcelCreator.Application.Tables;
using ExcelCreator.Core.Abstractions;
using ExcelCreator.Core.Models;
using ExcelCreator.Core.Validation;

namespace ExcelCreator.Application.Export;

public sealed class ExcelExportFacade : IExcelExportFacade
{
    private readonly IExcelExporter _exporter;
    private readonly IFileExportDialogService _dialogs;
    private readonly IUserSettingsStore _settings;
    private readonly IPersonnelRepository _personnel;
    private readonly IAppLogger _logger;

    public ExcelExportFacade(
        IExcelExporter exporter,
        IFileExportDialogService dialogs,
        IUserSettingsStore settings,
        IPersonnelRepository personnel,
        IAppLogger logger)
    {
        _exporter = exporter;
        _dialogs = dialogs;
        _settings = settings;
        _personnel = personnel;
        _logger = logger;
    }

    public bool ExportWithDialog(
        Window owner,
        TemplateDefinition template,
        IReadOnlyList<TableRow> rows,
        DateCalendarKind calendar,
        string suggestedFileName)
    {
        var sheet = template.RequirePrimarySheet();
        var validation = TableValidator.ValidateRows(sheet, rows, calendar);
        if (!validation.IsValid)
        {
            _dialogs.NotifyValidationError(validation.Message);
            return false;
        }

        var appSettings = _settings.Load();
        if (!_dialogs.TryGetSavePath(owner, suggestedFileName, appSettings.DefaultSaveFolder, out var path))
            return false;

        try
        {
            var normalized = TableValidator.NormalizeRows(sheet, rows);
            var resolvedTemplate = ColumnDropdownResolver.ResolveTemplate(template, _personnel);
            _exporter.Export(new GenerationRequest
            {
                Template = TableRowMapper.WithTimestampColumn(resolvedTemplate),
                UserRows = TableRowMapper.ToExportValues(normalized, calendar),
                DateCalendar = calendar
            }, path);

            appSettings.DefaultSaveFolder = Path.GetDirectoryName(path);
            _settings.Save(appSettings);
            _dialogs.NotifyExportSuccess(path, appSettings.OpenAfterCreate);
            _logger.Info($"Exported workbook to {path}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error("Export failed", ex);
            _dialogs.NotifyError(ex.Message);
            return false;
        }
    }
}
