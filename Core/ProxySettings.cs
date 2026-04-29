using System;
using System.IO;
using System.Text.Json;

public class ProxySettings
{
    private static readonly string ConfigFilePath = Path.Combine(AppContext.BaseDirectory, "proxyconfig.json");

    public bool EnableDamageLogging { get; set; } = false;
    public bool EnableDeathBeep { get; set; } = false;

    private static ProxySettings? _current;

    public static ProxySettings Current
    {
        get
        {
            if (_current == null)
            {
                LoadFromFile();
            }
            return _current!;
        }
    }

    /// <summary>
    /// Загрузить настройки из файла конфига
    /// </summary>
    private static void LoadFromFile()
    {
        try
        {
            if (File.Exists(ConfigFilePath))
            {
                string json = File.ReadAllText(ConfigFilePath);
                _current = JsonSerializer.Deserialize<ProxySettings>(json);
            }
        }
        catch
        {
            // Ignore load errors, use defaults
        }

        _current ??= new ProxySettings();
    }

    /// <summary>
    /// Сохранить настройки в файл конфига
    /// </summary>
    public void SaveToFile()
    {
        try
        {
            Directory.CreateDirectory(AppContext.BaseDirectory);
            string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigFilePath, json);
        }
        catch
        {
            // Ignore save errors to avoid breaking game flow
        }
    }
}
