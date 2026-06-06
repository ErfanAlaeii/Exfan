using System.IO;
using System.Text.Json;

namespace ExcelCreator.Infrastructure.Persistence;

public sealed class AtomicJsonStore<T> where T : new()
{
    private readonly string _path;
    private readonly JsonSerializerOptions _options;
    private readonly string _corruptFileLabel;
    private readonly Mutex _mutex;

    public AtomicJsonStore(string path, JsonSerializerOptions options, string corruptFileLabel)
    {
        _path = path;
        _options = options;
        _corruptFileLabel = corruptFileLabel;
        var mutexName = $"Global\\Exfan_{path.GetHashCode():X8}";
        _mutex = new Mutex(false, mutexName);
    }

    public T LoadOrDefault(Func<T>? factory = null)
    {
        factory ??= () => new T();
        _mutex.WaitOne();
        try
        {
            if (!File.Exists(_path))
                return factory();

            try
            {
                var json = File.ReadAllText(_path);
                return JsonSerializer.Deserialize<T>(json, _options) ?? factory();
            }
            catch (Exception ex)
            {
                var backup = _path + $".corrupt-{DateTime.UtcNow:yyyyMMddHHmmss}";
                try { File.Copy(_path, backup, overwrite: false); } catch { /* best effort */ }
                throw new InvalidOperationException(
                    $"فایل {_corruptFileLabel} خراب است. نسخه پشتیبان: {backup}", ex);
            }
        }
        finally
        {
            _mutex.ReleaseMutex();
        }
    }

    public void Save(T data)
    {
        _mutex.WaitOne();
        try
        {
            var directory = Path.GetDirectoryName(_path);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            var json = JsonSerializer.Serialize(data, _options);
            var tempPath = _path + ".tmp";
            File.WriteAllText(tempPath, json);
            File.Move(tempPath, _path, overwrite: true);
        }
        finally
        {
            _mutex.ReleaseMutex();
        }
    }
}
