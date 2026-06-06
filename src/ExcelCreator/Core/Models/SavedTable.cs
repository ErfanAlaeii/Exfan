using System.Text.Json.Serialization;
using TableRowListJsonConverter = ExcelCreator.Infrastructure.Persistence.TableRowListJsonConverter;

namespace ExcelCreator.Core.Models;

public sealed class SavedTable
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    [JsonPropertyName("templateId")]
    public string TemplateId { get; set; } = string.Empty;

    [JsonPropertyName("templateVersion")]
    public int TemplateVersion { get; set; } = 1;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("dateCalendar")]
    public DateCalendarKind DateCalendar { get; set; } = DateCalendarKind.Jalali;

    [JsonPropertyName("sheetName")]
    public string SheetName { get; set; } = string.Empty;

    [JsonPropertyName("columnHeaders")]
    public List<string> ColumnHeaders { get; set; } = [];

    [JsonPropertyName("rows")]
    [JsonConverter(typeof(TableRowListJsonConverter))]
    public List<TableRow> Rows { get; set; } = [];

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
