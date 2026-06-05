using System.IO;
using System.Text.Json;
using ExcelCreator.Abstractions;
using ExcelCreator.Infrastructure;
using ExcelCreator.Models;
using ExcelCreator.Validation;

namespace ExcelCreator.Services;

public sealed class TemplateService : ITemplateRepository
{
    private readonly string _templatesDirectory;
    private readonly IAppLogger _logger;
    private IReadOnlyList<TemplateDefinition>? _cache;

    public TemplateService(IAppLogger logger, string? templatesDirectory = null)
    {
        _logger = logger;
        _templatesDirectory = templatesDirectory
            ?? Path.Combine(AppContext.BaseDirectory, "Templates");
    }

    public IReadOnlyList<TemplateDefinition> LoadAll()
    {
        if (_cache is not null)
            return _cache;

        if (!Directory.Exists(_templatesDirectory))
        {
            _cache = [];
            return _cache;
        }

        var templates = new List<TemplateDefinition>();

        foreach (var file in Directory.EnumerateFiles(_templatesDirectory, "*.json"))
        {
            try
            {
                var json = File.ReadAllText(file);
                var template = JsonSerializer.Deserialize<TemplateDefinition>(json, JsonDefaults.Templates);
                if (template is null || string.IsNullOrWhiteSpace(template.Id))
                    continue;

                var validation = TemplateValidator.Validate(template);
                if (!validation.IsValid)
                {
                    _logger.Warning($"Skipped template {file}: {validation.Message}");
                    continue;
                }

                templates.Add(template);
            }
            catch (Exception ex)
            {
                _logger.Warning($"Skipped invalid template file {file}: {ex.Message}");
            }
        }

        _cache = templates
            .OrderBy(t => t.Category)
            .ThenBy(t => t.Title)
            .ToList();

        return _cache;
    }

    public TemplateDefinition? GetById(string id) =>
        LoadAll().FirstOrDefault(t => string.Equals(t.Id, id, StringComparison.OrdinalIgnoreCase));

    public void InvalidateCache() => _cache = null;
}
