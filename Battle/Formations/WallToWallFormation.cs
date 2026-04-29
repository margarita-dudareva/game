using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Боевое построение: стенка на стенку
/// Все юниты выстраиваются в один ряд
/// На ринге: юниты, у которых есть противник
/// В запасе: юниты без пары (если одна армия больше другой)
/// </summary>
public class WallToWallFormation : IBattleFormation
{
    public string Name => "Стенка на стенку";

    public List<IUnit> GetRingUnits(IArmy army)
    {
        var allAlive = army.Units.Where(u => u.IsAlive).ToList();
        
        // На ринге юниты, у которых есть противник ИЛИ которые находятся на позициях до максимального
        // Для этого нужно знать размер обеих армий... вернём все живые, потом фильтрация будет в GetOpponent
        return allAlive;
    }

    public List<IUnit> GetReserveUnits(IArmy army)
    {
        // В этом режиме "запас" - это те, у кого нет противника
        // Но мы это определим через GetOpponent, вернём пусто здесь
        return new List<IUnit>();
    }

    public bool IsOnRing(IUnit unit, IArmy army)
    {
        // Юнит на ринге, если он живой и имеет индекс < размера меньшей армии
        // Нужна информация о противнике...
        return unit.IsAlive;
    }

    public IUnit? GetOpponent(IUnit unit, IArmy army, IArmy opponentArmy)
    {
        if (!unit.IsAlive)
            return null;

        int myIndex = army.Units.IndexOf(unit);
        
        // Противник имеет такой же индекс в его армии, если такой существует
        if (myIndex >= 0 && myIndex < opponentArmy.Units.Count)
        {
            var opponent = opponentArmy.Units[myIndex];
            return opponent.IsAlive ? opponent : null;
        }

        return null;
    }

    public List<IUnit> GetAdjacentUnits(IUnit unit, IArmy army)
    {
        if (!unit.IsAlive)
            return new List<IUnit>();

        int myIndex = army.Units.IndexOf(unit);
        if (myIndex < 0)
            return new List<IUnit>();

        var adjacent = new List<IUnit>();

        // На вертикальной стенке соседи - это юниты выше/ниже по индексу
        var allLiving = army.Units.Where(u => u.IsAlive).ToList();

        for (int offset = -1; offset <= 1; offset++)
        {
            if (offset == 0)
                continue;

            int neighborIndex = myIndex + offset;
            if (neighborIndex >= 0 && neighborIndex < army.Units.Count)
            {
                var neighbor = army.Units[neighborIndex];
                if (neighbor.IsAlive)
                {
                    adjacent.Add(neighbor);
                }
            }
        }

        return adjacent;
    }

    public void HandleUnitDeath(IUnit deadUnit, IArmy army, IArmy opponentArmy)
    {
        // Просто удаляем мёртвого юнита
        // Остальные автоматически "сдвигаются" вверх благодаря индексам
        army.RemoveUnit(deadUnit);
        
        // При необходимости можно добавить логику проверки целостности фронта
        // но в текущей схеме это не требуется
    }

    public void PromoteUnitToRing(IUnit unit, IArmy army)
    {
        // На стенке на стенку нет явного "поднятия"
    }

    public (int row, int column) GetUnitPosition(IUnit unit, IArmy army)
    {
        int index = army.Units.IndexOf(unit);
        
        // Все в одной колонне (column = 0), row = индекс
        return (row: index, column: 0);
    }

    public List<IUnit> GetUnitsInRadius(IUnit unit, IArmy army, int radius)
    {
        if (!unit.IsAlive)
            return new List<IUnit>();

        int myIndex = army.Units.IndexOf(unit);
        if (myIndex < 0)
            return new List<IUnit>();

        var inRadius = new List<IUnit>();
        var allLiving = army.Units.Where(u => u.IsAlive).ToList();

        foreach (var other in allLiving)
        {
            if (other == unit)
                continue;

            int otherIndex = army.Units.IndexOf(other);
            
            // Расстояние - это разница индексов
            int distance = Math.Abs(myIndex - otherIndex);

            if (distance <= radius && distance > 0)
            {
                inRadius.Add(other);
            }
        }

        return inRadius;
    }
}
