using System;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Наблюдатель, логирующий урон в файл
/// </summary>
public class DamageLoggerObserver : IUnitEventObserver
{
    private static string GetLogFilePath()
    {
        int sessionNumber = DamageLogViewer.GetSessionNumber();
        return Path.Combine(AppContext.BaseDirectory, $"damage_log_session_{sessionNumber}.txt");
    }

    public void OnDamageTaken(DamageTakenEventArgs args)
    {
        try
        {
            Directory.CreateDirectory(AppContext.BaseDirectory);
            string logFilePath = GetLogFilePath();
            File.AppendAllText(logFilePath,
                $"[{args.Timestamp:yyyy-MM-dd HH:mm:ss.fff}] Юнит {args.Unit.Name} (ID {args.Unit.Id}) получил {args.DamageTaken} урона (с {args.HealthBefore} на {args.HealthAfter})\n");
        }
        catch
        {
            // Игнорируем ошибки логирования, чтобы не прерывать ход игры
        }
    }

    public void OnUnitDeath(UnitDeathEventArgs args)
    {
        // Для логгера урона смерть не требует специальной обработки
    }
}
