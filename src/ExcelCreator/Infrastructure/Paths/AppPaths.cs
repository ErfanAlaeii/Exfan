using System.IO;

namespace ExcelCreator.Infrastructure.Paths;

public static class AppPaths
{
    private const string LegacyFolderName = "ExcelCreator";
    private const string AppFolderName = "Exfan";

    public static string AppDataDirectory
    {
        get
        {
            var root = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var current = Path.Combine(root, AppFolderName);
            var legacy = Path.Combine(root, LegacyFolderName);

            Directory.CreateDirectory(current);

            if (!Directory.Exists(legacy))
                return current;

            MigrateFileIfMissing(legacy, current, "settings.json");
            MigrateFileIfMissing(legacy, current, "tables.json");
            MigrateFileIfMissing(legacy, current, "personnel.json");

            return current;
        }
    }

    public static string SettingsFile => Path.Combine(AppDataDirectory, "settings.json");

    public static string TablesFile => Path.Combine(AppDataDirectory, "tables.json");

    public static string PersonnelFile => Path.Combine(AppDataDirectory, "personnel.json");

    public static string ImagesDirectory
    {
        get
        {
            var path = Path.Combine(AppDataDirectory, "images");
            Directory.CreateDirectory(path);
            return path;
        }
    }

    private static void MigrateFileIfMissing(string legacyDir, string currentDir, string fileName)
    {
        var legacyPath = Path.Combine(legacyDir, fileName);
        var currentPath = Path.Combine(currentDir, fileName);
        if (File.Exists(legacyPath) && !File.Exists(currentPath))
            File.Copy(legacyPath, currentPath);
    }
}
