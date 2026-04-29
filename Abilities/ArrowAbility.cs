using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Special ability для лучника - стрельба из лука
/// Может активироваться только если лучник не стоит первым в армии
/// </summary>
public class ArrowAbility : ISpecialAbility
{
    public string Name => "Arrow Attack";

    private int _range;
    private int _power;
    private int _unitPosition;
    private IUnit? _archer;

    public int Range => _range;
    public int Power => _power;

    public ArrowAbility(int range, int power)
    {
        _range = range;
        _power = power;
        _unitPosition = 0;
        _archer = null;
    }

    public bool CanActivate(IUnit target)
    {
        // Лучник может стрелять только если он не первый и если дальность > кол-ва человек перед ним
        if (_unitPosition == 0)
            return false;

        // Количество человек перед лучником = его позиция и если дальность > кол-во перед ним, то может стрелять
        return _range > _unitPosition;
    }

    /// <summary>
    /// Активирует способность стрельбы из лука, нанося урон целевому юниту с учетом защиты
    /// </summary>
    public void Activate(IUnit target)
    {
        int actualDamage = Math.Max(0, _power - target.Defense);
        
        // Если цель имеет защиту от стрел, уменьшаем урон
        if (target is BuffableHeavyInfantry buffableTarget)
        {
            actualDamage = buffableTarget.CalculateDamageWithArrowDefense(actualDamage);
        }
        
        target.TakeDamage(actualDamage);
    }

    /// <summary>
    /// Установить позицию юнита в армии (вызывается из Battle)
    /// </summary>
    public void SetUnitPosition(int position)
    {
        _unitPosition = position;
    }

    /// <summary>
    /// Установить самого лучника (юнита с этой способностью)
    /// </summary>
    public void SetArcher(IUnit archer)
    {
        _archer = archer;
    }

    /// <summary>
    /// Получить список доступных целей из армии противника с учётом боевого построения
    /// </summary>
    public List<IUnit> GetAvailableTargets(IArmy opponentArmy, IBattleFormation? formation = null, IArmy? archerArmy = null)
    {
        var availableTargets = new List<IUnit>();

        if (_unitPosition == 0)
            return availableTargets;

        // Если есть formation и это 3 на 3, используем радиус на сетке
        if (formation is ThreeOnThreeFormation && _archer != null && archerArmy != null)
        {
            // Используем GetUnitsInRadius для определения целей по двумерной сетке
            var unitsInRange = formation.GetUnitsInRadius(_archer, opponentArmy, _range);
            return unitsInRange.Where(u => u.IsAlive).ToList();
        }

        // Для режима 1 на 1 (Bridge) или если formation не задана: старая логика
        int maxReachablePosition = Math.Min(_range - _unitPosition, opponentArmy.Units.Count);

        for (int i = 0; i < maxReachablePosition && i < opponentArmy.Units.Count; i++)
        {
            if (opponentArmy.Units[i].IsAlive)
            {
                availableTargets.Add(opponentArmy.Units[i]);
            }
        }

        return availableTargets;
    }
}