namespace ExcelCreator.Excel;

public sealed record ExcelImageReference(
    string PictureName,
    string HyperlinkTarget,
    bool IsExternal,
    string Tooltip);
