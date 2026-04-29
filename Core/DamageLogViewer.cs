using System;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// Просмотр и вывод логирования урона
/// </summary>
public class DamageLogViewer
{
    private static int _sessionNumber = 1;
    private static string LogFilePath => Path.Combine(AppContext.BaseDirectory, $"damage_log_session_{_sessionNumber}.txt");

    /// <summary>
    /// Установить номер сессии для логирования
    /// </summary>
    public static void SetSessionNumber(int sessionNumber)
    {
        _sessionNumber = sessionNumber;
    }

    /// <summary>
    /// Получить текущий номер сессии
    /// </summary>
    public static int GetSessionNumber()
    {
        return _sessionNumber;
    }

    /// <summary>
    /// Проверить, существует ли лог-файл с данными
    /// </summary>
    public static bool HasLogFile()
    {
        return File.Exists(LogFilePath) && new FileInfo(LogFilePath).Length > 0;
    }

    /// <summary>
    /// Вывести лог в консоль
    /// </summary>
    public static void DisplayLog()
    {
        if (!HasLogFile())
        {
            Console.WriteLine("\nЛог урона пуст или не существует.");
            return;
        }

        try
        {
            Console.Clear();
            Console.WriteLine(new string('=', 63));
            Console.WriteLine("ЛОГ УРОНА".PadLeft(47).PadRight(63));
            Console.WriteLine();

            var lines = File.ReadAllLines(LogFilePath);
            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    Console.WriteLine(line);
                }
            }

            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nОшибка при чтении лога: {ex.Message}");
        }
    }

    /// <summary>
    /// Инициализировать номер сессии (найти максимальный существующий лог и инкрементировать)
    /// </summary>
    public static void InitializeSessionNumber()
    {
        var logDir = AppContext.BaseDirectory;
        var existingLogs = Directory.GetFiles(logDir, "damage_log_session_*.txt");
        
        int maxSession = 0;
        foreach (var logFile in existingLogs)
        {
            var filename = Path.GetFileNameWithoutExtension(logFile);
            if (filename.StartsWith("damage_log_session_") && 
                int.TryParse(filename.Replace("damage_log_session_", ""), out int sessionNum))
            {
                maxSession = Math.Max(maxSession, sessionNum);
            }
        }
        
        _sessionNumber = maxSession + 1;
    }

    /// <summary>
    /// Очистить лог для новой игры
    /// </summary>
    public static void ClearLog()
    {
        try
        {
            // Инкрементировать номер сессии для новой игры
            _sessionNumber++;
            
            if (File.Exists(LogFilePath))
            {
                File.Delete(LogFilePath);
            }
        }
        catch
        {
            // Ignore errors on cleanup
        }
    }

    /// <summary>
    /// Получить статистику логирования (кол-во урона по юнитам)
    /// </summary>
    public static Dictionary<string, (int damageCount, int totalDamage)> GetDamageStatistics()
    {
        var statistics = new Dictionary<string, (int, int)>();

        if (!HasLogFile())
            return statistics;

        try
        {
            var lines = File.ReadAllLines(LogFilePath);
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                // Parse: "[time] Unit {Name} (ID {Id}) took {damage} damage (from {before} to {after})"
                if (line.Contains("took") && line.Contains("damage"))
                {
                    try
                    {
                        int startIdx = line.IndexOf("Unit ") + 5;
                        int endIdx = line.IndexOf(" (ID");
                        if (startIdx > 4 && endIdx > startIdx)
                        {
                            string unitName = line.Substring(startIdx, endIdx - startIdx);

                            int damageStartIdx = line.IndexOf("took ") + 5;
                            int damageEndIdx = line.IndexOf(" damage", damageStartIdx);
                            if (int.TryParse(line.Substring(damageStartIdx, damageEndIdx - damageStartIdx), out int damage))
                            {
                                if (statistics.ContainsKey(unitName))
                                {
                                    var (count, total) = statistics[unitName];
                                    statistics[unitName] = (count + 1, total + damage);
                                }
                                else
                                {
                                    statistics[unitName] = (1, damage);
                                }
                            }
                        }
                    }
                    catch
                    {
                        // Skip parsing errors for individual lines
                    }
                }
            }
        }
        catch
        {
            // Return empty statistics on error
        }

        return statistics;
    }
}
