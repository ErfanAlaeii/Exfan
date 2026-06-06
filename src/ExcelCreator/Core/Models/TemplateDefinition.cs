using System.Text.Json.Serialization;



namespace ExcelCreator.Core.Models;



public sealed class TemplateDefinition

{

    [JsonPropertyName("id")]

    public string Id { get; set; } = string.Empty;



    [JsonPropertyName("version")]

    public int Version { get; set; } = 1;



    [JsonPropertyName("title")]

    public string Title { get; set; } = string.Empty;



    [JsonPropertyName("description")]

    public string Description { get; set; } = string.Empty;



    [JsonPropertyName("category")]

    public string Category { get; set; } = "General";



    [JsonPropertyName("icon")]

    public string Icon { get; set; } = "📊";



    [JsonPropertyName("defaultFileName")]

    public string DefaultFileName { get; set; } = "Workbook";



    [JsonPropertyName("workbook")]

    public WorkbookSpec Workbook { get; set; } = new();

}



public sealed class WorkbookSpec

{

    [JsonPropertyName("sheets")]

    public List<SheetSpec> Sheets { get; set; } = [];

}



public sealed class SheetSpec

{

    [JsonPropertyName("name")]

    public string Name { get; set; } = "Sheet1";



    [JsonPropertyName("columns")]

    public List<ColumnSpec> Columns { get; set; } = [];



    [JsonPropertyName("features")]

    public List<string> Features { get; set; } = [];

}



public sealed class ColumnSpec

{

    [JsonPropertyName("header")]

    public string Header { get; set; } = string.Empty;



    [JsonPropertyName("type")]

    public string Type { get; set; } = ColumnTypes.Text;



    [JsonPropertyName("width")]

    public double? Width { get; set; }



    [JsonPropertyName("formula")]

    public string? Formula { get; set; }



    [JsonPropertyName("dropdownValues")]

    public List<string>? DropdownValues { get; set; }

}



public sealed class GenerationRequest

{

    public required TemplateDefinition Template { get; init; }

    public List<List<string>> UserRows { get; init; } = [];

    public DateCalendarKind DateCalendar { get; init; } = DateCalendarKind.Jalali;

}

