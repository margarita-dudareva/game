using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Управление коллекцией юнитов
/// </summary>
public class Army : IArmy
{
    public string Name { get; private set; }

    public List<IUnit> Units { get; private set; } = new List<IUnit>();

    public Army(string name)
    {
        Name = name;
    }

    public void AddUnit(IUnit unit)
    {
        if (unit != null)
            Units.Add(unit);
    }

    public void RemoveUnit(IUnit unit)
    {
        if (unit == null)
            return;

        Units.Remove(unit);
    }

    public bool HasAliveUnits()
    {
        return Units.Any(u => u.IsAlive);
    }

    public IUnit? GetCurrentUnit()
    {
        return Units.FirstOrDefault(u => u.IsAlive);
    }
 
    public void RemoveDeadUnits()
    {
        Units.RemoveAll(u => !u.IsAlive);
    }
}
