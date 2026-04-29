using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Боевое построение: мост (1 на 1)
/// Только первый живой юнит каждой армии находится на ринге
/// Остальные - в запасе и могут использовать спецсособности
/// </summary>
public class BridgeFormation : IBattleFormation
{
    public string Name => "Боевой мост (1 на 1)";

    public List<IUnit> GetRingUnits(IArmy army)
    {
        var ringUnit = army.Units.FirstOrDefault(u => u.IsAlive);
        return ringUnit != null ? new List<IUnit> { ringUnit } : new List<IUnit>();
    }

    public List<IUnit> GetReserveUnits(IArmy army)
    {
        var ringUnit = army.Units.FirstOrDefault(u => u.IsAlive);
        return army.Units.Where(u => u.IsAlive && u != ringUnit).ToList();
    }

    public bool IsOnRing(IUnit unit, IArmy army)
    {
        var ringUnit = army.Units.FirstOrDefault(u => u.IsAlive);
        return unit.IsAlive && unit == ringUnit;
    }

    public IUnit? GetOpponent(IUnit unit, IArmy army, IArmy opponentArmy)
    {
        // На мосту каждый боец имеет только одного противника - первого живого из его армии
        if (!IsOnRing(unit, army))
            return null;

        return opponentArmy.Units.FirstOrDefault(u => u.IsAlive);
    }

    public List<IUnit> GetAdjacentUnits(IUnit unit, IArmy army)
    {
        // На мосту соседей нет - каждый юнит сражается один
        return new List<IUnit>();
    }

    public void HandleUnitDeath(IUnit deadUnit, IArmy army, IArmy opponentArmy)
    {
        // Удаляем мёртвого юнита, следующий автоматически займёт его место
        // благодаря FirstOrDefault() в GetRingUnits()
        army.RemoveUnit(deadUnit);
    }

    public void PromoteUnitToRing(IUnit unit, IArmy army)
    {
        // На мосту нет явного повышения - всегда используется первый живой
        // Этот метод здесь не имеет смысла, но требуется интерфейсом
    }

    public (int row, int column) GetUnitPosition(IUnit unit, IArmy army)
    {
        // На мосту все единицы в одном ряду (row = 0)
        // column - это индекс в очереди
        int index = army.Units.IndexOf(unit);
        return (row: 0, column: index);
    }

    public List<IUnit> GetUnitsInRadius(IUnit unit, IArmy army, int radius)
    {
        // На мосту в радиусе нет никого
        return new List<IUnit>();
    }
}
