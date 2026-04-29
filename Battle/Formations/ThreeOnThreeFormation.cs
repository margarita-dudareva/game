using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Боевое построение: 3 на 3
/// Армия делится на 3 колонки
/// На ринге: первая колонка (row == 0)
/// В запасе: остальные колонки (row > 0)
/// </summary>
public class ThreeOnThreeFormation : IBattleFormation
{
    private const int UNITS_PER_ROW = 3;

    public string Name => "Три на три (3 на 3)";

    public List<IUnit> GetRingUnits(IArmy army)
    {
        // На ринге юниты с индексами 0, 1, 2 (первая колонка)
        return army.Units
            .Where(u => u.IsAlive && GetUnitPosition(u, army).row == 0)
            .ToList();
    }

    public List<IUnit> GetReserveUnits(IArmy army)
    {
        // В запасе юниты со строками > 0
        return army.Units
            .Where(u => u.IsAlive && GetUnitPosition(u, army).row > 0)
            .ToList();
    }

    public bool IsOnRing(IUnit unit, IArmy army)
    {
        return unit.IsAlive && GetUnitPosition(unit, army).row == 0;
    }

    public IUnit? GetOpponent(IUnit unit, IArmy army, IArmy opponentArmy)
    {
        if (!unit.IsAlive)
            return null;

        var unitPos = GetUnitPosition(unit, army);
        
        // На ринге ищем противника на той же колонке
        if (unitPos.row != 0)
            return null;

        // Ищем юнита противника на той же позиции (та же колонка)
        var opponents = opponentArmy.Units
            .Where(u => u.IsAlive && GetUnitPosition(u, opponentArmy).column == unitPos.column)
            .ToList();

        return opponents.FirstOrDefault();
    }

    public List<IUnit> GetAdjacentUnits(IUnit unit, IArmy army)
    {
        if (!unit.IsAlive)
            return new List<IUnit>();

        var unitPos = GetUnitPosition(unit, army);
        var adjacent = new List<IUnit>();

        // Получить все живые юниты
        var allLiving = army.Units.Where(u => u.IsAlive).ToList();

        foreach (var other in allLiving)
        {
            if (other == unit)
                continue;

            var otherPos = GetUnitPosition(other, army);

            // Сосед если разница по строкам и колонкам не более 1 (8 соседей включая диагонали)
            if (Math.Abs(unitPos.row - otherPos.row) <= 1 && 
                Math.Abs(unitPos.column - otherPos.column) <= 1)
            {
                adjacent.Add(other);
            }
        }

        return adjacent;
    }

    public void HandleUnitDeath(IUnit deadUnit, IArmy army, IArmy opponentArmy)
    {
        var deadPos = GetUnitPosition(deadUnit, army);
        
        // Удаляем мёртвого юнита
        army.RemoveUnit(deadUnit);

        // Если мёртвый был на ринге (row == 0), пытаемся поднять замену из запаса
        if (deadPos.row == 0)
        {
            // Ищем живого юнита на той же колонке в строке row+1
            var replacement = army.Units
                .Where(u => u.IsAlive)
                .FirstOrDefault(u => 
                {
                    var pos = GetUnitPosition(u, army);
                    return pos.column == deadPos.column && pos.row > 0;
                });

            // Если нашли замену, переместим всех юнитов из первого ряда на место или оставим как есть
            // На самом деле, благодаря использованию индексов в GetUnitPosition,
            // пересчёт позиций произойдёт автоматически после удаления
        }
    }

    public void PromoteUnitToRing(IUnit unit, IArmy army)
    {
        // На режиме 3 на 3 движение происходит автоматически
        // через пересчёт позиций по индексам
    }

    public (int row, int column) GetUnitPosition(IUnit unit, IArmy army)
    {
        int index = army.Units.IndexOf(unit);
        if (index < 0)
            return (row: -1, column: -1);

        int row = index / UNITS_PER_ROW;
        int column = index % UNITS_PER_ROW;
        
        return (row, column);
    }

    public List<IUnit> GetUnitsInRadius(IUnit unit, IArmy army, int radius)
    {
        if (!unit.IsAlive)
            return new List<IUnit>();

        var unitPos = GetUnitPosition(unit, army);
        var inRadius = new List<IUnit>();

        var allLiving = army.Units.Where(u => u.IsAlive).ToList();

        foreach (var other in allLiving)
        {
            if (other == unit)
                continue;

            var otherPos = GetUnitPosition(other, army);

            // Расстояние в "шахматном" порядке (Chebyshev distance)
            int distance = Math.Max(Math.Abs(unitPos.row - otherPos.row), 
                                   Math.Abs(unitPos.column - otherPos.column));

            if (distance <= radius && distance > 0)
            {
                inRadius.Add(other);
            }
        }

        return inRadius;
    }
}
