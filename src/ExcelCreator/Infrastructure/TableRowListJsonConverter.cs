using System.Text.Json;
using System.Text.Json.Serialization;
using ExcelCreator.Models;

namespace ExcelCreator.Infrastructure;

public sealed class TableRowListJsonConverter : JsonConverter<List<TableRow>>
{
    public override List<TableRow> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException("Expected JSON array for table rows.");

        var rows = new List<TableRow>();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                return rows;

            if (reader.TokenType == JsonTokenType.StartArray)
            {
                var values = JsonSerializer.Deserialize<List<string>>(ref reader, options) ?? [];
                rows.Add(TableRow.FromValues(values, DateTime.UtcNow));
                continue;
            }

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                var row = JsonSerializer.Deserialize<TableRow>(ref reader, options);
                if (row is not null)
                    rows.Add(row);
                continue;
            }

            throw new JsonException($"Unexpected token {reader.TokenType} in table rows.");
        }

        throw new JsonException("Unexpected end while reading table rows.");
    }

    public override void Write(Utf8JsonWriter writer, List<TableRow> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (var row in value)
            JsonSerializer.Serialize(writer, row, options);
        writer.WriteEndArray();
    }
}
