using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Special ability для лекаря - лечение союзников
/// Лекарь может атаковать, но вместо нанесения урона лечит союзников в радиусе действия
/// Лечение восстанавливает HP до максимального, но не превышает исходное значение
/// </summary>
public class HealAbility : ISpecialAbility
{
    public string Name => "Heal";

    private int _range;
    private int _healPower;
    private int _unitPosition;
    private IUnit? _healer;

    public int Range => _range;
    public int HealPower => _healPower;

    public HealAbility(int range, int healPower)
    {
        _range = range;
        _healPower = healPower;
        _unitPosition = 0;
        _healer = null;
    }

    public bool CanActivate(IUnit target)
    {
        // Лекарь может лечить всегда, если он в армии
        return target is ICanBeHealed;
    }

    /// <summary>
    /// Активирует способность лечения, восстанавливая HP целевому юниту
    /// </summary>
    public void Activate(IUnit target)
    {
        if (target is ICanBeHealed)
        {
            int healAmount = Math.Min(_healPower, target.MaxHealth - target.CurrentHealth);
            target.Heal(healAmount);
        }
    }

    /// <summary>
    /// Установить позицию юнита в армии (вызывается из Battle)
    /// </summary>
    public void SetUnitPosition(int position)
    {
        _unitPosition = position;
    }

    /// <summary>
    /// Установить самого лекаря (юнита с этой способностью)
    /// </summary>
    public void SetHealer(IUnit healer)
    {
        _healer = healer;
    }

    /// <summary>
    /// Получить список доступных целей для лечения из дружественной армии с учётом боевого построения
    /// </summary>
    public List<IUnit> GetAvailableTargets(IArmy friendlyArmy, IBattleFormation? formation = null)
    {
        var availableTargets = new List<IUnit>();

        if (_unitPosition == 0)
            return availableTargets;

        // Если есть formation и это 3 на 3, используем радиус на сетке
        if (formation is ThreeOnThreeFormation && _healer != null)
        {
            // Используем GetUnitsInRadius для определения целей по двумерной сетке
            var unitsInRange = formation.GetUnitsInRadius(_healer, friendlyArmy, _range);
            return unitsInRange.Where(u => u.IsAlive && u is ICanBeHealed && u.CurrentHealth < u.MaxHealth).ToList();
        }

        // Для режима 1 на 1 (Bridge) или если formation не задана: старая логика
        int maxReachableRange = _range;

        for (int i = 0; i < friendlyArmy.Units.Count; i++)
        {
            var unit = friendlyArmy.Units[i];
            if (unit is ICanBeHealed && unit.IsAlive && unit.CurrentHealth < unit.MaxHealth)
            {
                // Простая проверка расстояния - юнит должен быть достаточно близко
                int distance = Math.Abs(i - _unitPosition);
                if (distance <= maxReachableRange)
                {
                    availableTargets.Add(unit);
                }
            }
        }

        return availableTargets;
    }
}


//////sssss