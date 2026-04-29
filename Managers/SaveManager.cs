using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

/// <summary>
/// Управление сохранениями игры в JSON
/// </summary>
public class SaveManager
{
    private const string SaveDirectory = "saves";
    private const string SaveExtension = ".json";

    public SaveManager()
    {
        if (!Directory.Exists(SaveDirectory))
        {
            Directory.CreateDirectory(SaveDirectory);
        }
    }

    /// <summary>
    /// Сохранить игру
    /// </summary>
    public void SaveGame(GameSaveData saveData, string fileName)
    {
        try
        {
            string filePath = Path.Combine(SaveDirectory, fileName + SaveExtension);
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(saveData, options);
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Ошибка при сохранении: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Загрузить игру
    /// </summary>
    public GameSaveData? LoadGame(string fileName)
    {
        try
        {
            string filePath = Path.Combine(SaveDirectory, fileName + SaveExtension);
            if (!File.Exists(filePath))
            {
                return null;
            }

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<GameSaveData>(json);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Ошибка при загрузке: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Получить список всех сохраненных игр
    /// </summary>
    public List<string> GetSavesList()
    {
        try
        {
            if (!Directory.Exists(SaveDirectory))
                return new List<string>();

            var files = Directory.GetFiles(SaveDirectory, $"*{SaveExtension}")
                .Select(f => Path.GetFileNameWithoutExtension(f))
                .ToList();

            return files;
        }
        catch (Exception)
        {
            return new List<string>();
        }
    }

    /// <summary>
    /// Удалить сохраненную игру
    /// </summary>
    public void DeleteGame(string fileName)
    {
        try
        {
            string filePath = Path.Combine(SaveDirectory, fileName + SaveExtension);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Ошибка при удалении сохранения: {ex.Message}", ex);
        }
    }
}
