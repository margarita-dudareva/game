/// <summary>
/// Событие, срабатывающее при получении урона юнитом
/// </summary>
public class DamageTakenEventArgs
{
    public IUnit Unit { get; set; }
    public int DamageTaken { get; set; }
    public int HealthBefore { get; set; }
    public int HealthAfter { get; set; }
    public DateTime Timestamp { get; set; }

    public DamageTakenEventArgs(IUnit unit, int damageTaken, int healthBefore, int healthAfter)
    {
        Unit = unit;
        DamageTaken = damageTaken;
        HealthBefore = healthBefore;
        HealthAfter = healthAfter;
        Timestamp = DateTime.Now;
    }
}

/// <summary>
/// Событие смерти юнита
/// </summary>
public class UnitDeathEventArgs
{
    public IUnit Unit { get; set; }
    public DateTime Timestamp { get; set; }

    public UnitDeathEventArgs(IUnit unit)
    {
        Unit = unit;
        Timestamp = DateTime.Now;
    }
}

/// <summary>
/// Интерфейс для наблюдателей за событиями юнита
/// </summary>
public interface IUnitEventObserver
{
    /// <summary>
    /// Вызывается когда юнит получает урон
    /// </summary>
    void OnDamageTaken(DamageTakenEventArgs args);

    /// <summary>
    /// Вызывается когда юнит умирает
    /// </summary>
    void OnUnitDeath(UnitDeathEventArgs args);
}
