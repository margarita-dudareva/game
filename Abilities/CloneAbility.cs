using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Special ability для волшебника - клонирование юнитов
/// С маленькой вероятностью может создать копию выбранного союзного юнита
/// Клон наследует текущие параметры исходного юнита (включая текущий HP)
/// </summary>
public class CloneAbility : ISpecialAbility
{
    public string Name => "Clone";

    private int _range;
    private int _cloneProbability; // вероятность в % (например 35 = 35%)
    private int _unitPosition;
    private Random _random;
    private IUnit? _wizard;

    public int Range => _range;
    public int CloneProbability => _cloneProbability;

    public CloneAbility(int range, int cloneProbability = 35)
    {
        _range = range;
        _cloneProbability = Math.Max(10, Math.Min(100, cloneProbability)); // Ограничиваем 10-100%
        _unitPosition = 0;
        _random = new Random();
        _wizard = null;
    }

    public bool CanActivate(IUnit target)
    {
        // Волшебник может клонировать союзных юнитов (пехоту и лучников)
        return (target is LightInfantry || target is HeavyInfantry || target is BuffableHeavyInfantry ||
                (target.SpecialAbility is ArrowAbility)) && target.IsAlive;
    }

    /// <summary>
    /// Активирует способность клонирования (для интерфейса ISpecialAbility)
    /// </summary>
    public void Activate(IUnit target)
    {
        // Не используется напрямую, используется TryClone() вместо этого
    }

    /// <summary>
    /// Пытается клонировать юнита
    /// Возвращает true если клонирование успешно, false если не удалось по вероятности
    /// </summary>
    public bool TryClone()
    {
        // Проверяем вероятность успеха
        return _random.Next(100) < _cloneProbability;
    }

    /// <summary>
    /// Установить позицию юнита в армии
    /// </summary>
    public void SetUnitPosition(int position)
    {
        _unitPosition = position;
    }

    /// <summary>
    /// Установить самого волшебника (юнита с этой способностью)
    /// </summary>
    public void SetWizard(IUnit wizard)
    {
        _wizard = wizard;
    }

    /// <summary>
    /// Получить список доступных целей для клонирования из дружественной армии с учётом боевого построения
    /// </summary>
    public List<IUnit> GetAvailableTargets(IArmy friendlyArmy, IBattleFormation? formation = null)
    {
        var availableTargets = new List<IUnit>();

        if (_unitPosition == 0)
            return availableTargets;

        // Если есть formation и это 3 на 3, используем радиус на сетке
        if (formation is ThreeOnThreeFormation && _wizard != null)
        {
            // Используем GetUnitsInRadius для определения целей по двумерной сетке
            var unitsInRange = formation.GetUnitsInRadius(_wizard, friendlyArmy, _range);
            return unitsInRange.Where(u => (u is LightInfantry || u is HeavyInfantry || u is BuffableHeavyInfantry || 
                                            (u.SpecialAbility is ArrowAbility)) && u.IsAlive && u != _wizard).ToList();
        }

        // Для режима 1 на 1 (Bridge) или если formation не задана: старая логика
        int maxReachableRange = _range;

        for (int i = 0; i < friendlyArmy.Units.Count; i++)
        {
            var unit = friendlyArmy.Units[i];
            
            // Может клонировать только пехоту и лучников, не себя
            bool canClone = (unit is LightInfantry || unit is HeavyInfantry || unit is BuffableHeavyInfantry ||
                            (unit.SpecialAbility is ArrowAbility)) && 
                           unit.IsAlive && unit != _wizard;
            
            if (canClone)
            {
                int distance = Math.Abs(i - _unitPosition);
                if (distance <= maxReachableRange)
                {
                    availableTargets.Add(unit);
                }
            }
        }

        return availableTargets;
    }

    /// <summary>
    /// Создать клона юнита с текущими параметрами
    /// </summary>
    public IUnit? CreateClone(IUnit targetUnit)
    {
        IUnit? clone = null;

        if (targetUnit is LightInfantry targetLight)
        {
            // Создаем точную копию с теми же параметрами
            var newLightInfantry = new LightInfantry(targetUnit.Attack, targetUnit.Defense, 
                                                      targetUnit.MaxHealth, targetUnit.Cost);
            newLightInfantry.SetCurrentHealth(targetUnit.CurrentHealth);
            clone = newLightInfantry;
        }
        else if (targetUnit is BuffableHeavyInfantry targetBuffableHeavy)
        {
            // Создаем копию BuffableHeavyInfantry с теми же параметрами
            var newBuffableHeavyInfantry = new BuffableHeavyInfantry(targetUnit.Attack, targetUnit.Defense, 
                                                      targetUnit.MaxHealth, targetUnit.Cost);
            newBuffableHeavyInfantry.SetCurrentHealth(targetUnit.CurrentHealth);
            
            // Копируем все буффы
            foreach (var buff in targetBuffableHeavy.GetActiveBuffs())
            {
                newBuffableHeavyInfantry.AddBuff(buff);
            }
            
            clone = newBuffableHeavyInfantry;
        }
        else if (targetUnit is HeavyInfantry targetHeavy)
        {
            // Создаем точную копию с теми же параметрами (старый тип, если еще остал)
            var newHeavyInfantry = new HeavyInfantry(targetUnit.Attack, targetUnit.Defense, 
                                                      targetUnit.MaxHealth, targetUnit.Cost);
            newHeavyInfantry.SetCurrentHealth(targetUnit.CurrentHealth);
            clone = newHeavyInfantry;
        }
        else if (targetUnit.SpecialAbility is ArrowAbility targetArrow)
        {
            // Для лучника копируем все параметры точно
            var newArcher = new UnitBase("Archer", targetUnit.Attack, targetUnit.Defense, 
                                        targetUnit.MaxHealth, targetUnit.Cost, 
                                        new ArrowAbility(targetArrow.Range, targetArrow.Power));
            newArcher.SetCurrentHealth(targetUnit.CurrentHealth);
            clone = newArcher;
        }

        return clone;
    }

    /// <summary>
    /// Устанавливает текущее здоровье клону (то же, что и у оригинала)
    /// </summary>
    private void SetCloneHealth(IUnit clone, int desiredHealth)
    {
        clone.SetCurrentHealth(desiredHealth);
    }
}
