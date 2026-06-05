using System.Text.Json.Serialization;

namespace ExcelCreator.Models;

public sealed class TableRow
{
    [JsonPropertyName("values")]
    public List<string> Values { get; set; } = [];

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public static TableRow CreateNew(IEnumerable<string> values) => new()
    {
        Values = values.ToList(),
        CreatedAt = DateTime.UtcNow
    };

    public static TableRow FromValues(IEnumerable<string> values, DateTime createdAt) => new()
    {
        Values = values.ToList(),
        CreatedAt = createdAt
    };
}
