using System;

/// <summary>
/// Наблюдатель, издающий звук при смерти юнита
/// </summary>
public class DeathBeepObserver : IUnitEventObserver
{
    public void OnDamageTaken(DamageTakenEventArgs args)
    {
        // Для звука при смерти мы реагируем только на OnUnitDeath
    }

    public void OnUnitDeath(UnitDeathEventArgs args)
    {
        try
        {
            if (OperatingSystem.IsWindows())
                Console.Beep(800, 150);

            Console.WriteLine($"[Observer] {args.Unit.Name} (ID {args.Unit.Id}) умер, бип!\n");
        }
        catch
        {
            // Если сигнал не поддерживается или ошибка - игнорируем
        }
    }
}
