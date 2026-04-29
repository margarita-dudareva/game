using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Special ability для оруженосца (Light Infantry)
/// С маленькой вероятностью применяет случайный бафф соседнему тяжелому воину
/// </summary>
public class SquireAbility : ISpecialAbility
{
    public string Name => "Squire";

    private int _activationProbability; // вероятность в % (например 25 = 25%)
    private int _unitPosition;
    private Random _random;
    private IUnit? _squire;

    public int ActivationProbability => _activationProbability;

    public SquireAbility(int activationProbability = 25)
    {
        _activationProbability = Math.Max(10, Math.Min(50, activationProbability)); // Ограничиваем 10-50%
        _unitPosition = 0;
        _random = new Random();
        _squire = null;
    }

    public bool CanActivate(IUnit target)
    {
        // Оруженосец может активировать способность, если рядом есть тяжёлый воин
        return (target is IBuffable buffableUnit && buffableUnit is BuffableHeavyInfantry && target.IsAlive);
    }

    /// <summary>
    /// Активирует способность применения баффа
    /// </summary>
    public void Activate(IUnit target)
    {
        if (target is IBuffable buffableUnit)
        {
            // Генерируем N (бонус баффа) от 1 до 5
            int buffBonus = _random.Next(1, 6);
            
            // Выбираем случайный бафф
            IBuff buff = GetRandomBuff(buffBonus);
            
            // Применяем бафф
            buffableUnit.AddBuff(buff);
        }
    }

    /// <summary>
    /// Возвращает случайный бафф с заданным бонусом
    /// </summary>
    private IBuff GetRandomBuff(int buffBonus)
    {
        int buffType = _random.Next(4); // 0-3 для 4 типов баффов
        
        return buffType switch
        {
            0 => new ShieldBuff(buffBonus),      // Shield - защита
            1 => new SpearBuff(buffBonus),       // Spear - атака
            2 => new HorseBuff(buffBonus),       // Horse - атака + защита
            3 => new HelmetBuff(buffBonus),      // Helmet - защита от стрел (используем переданный бонус как multiplier)
            _ => new ShieldBuff(buffBonus)
        };
    }

    /// <summary>
    /// Пытается активировать способность
    /// </summary>
    public bool TryActivate()
    {
        return _random.Next(100) < _activationProbability;
    }

    /// <summary>
    /// Установить позицию юнита в армии
    /// </summary>
    public void SetUnitPosition(int position)
    {
        _unitPosition = position;
    }

    /// <summary>
    /// Установить самого оруженосца (юнита с этой способностью)
    /// </summary>
    public void SetSquire(IUnit squire)
    {
        _squire = squire;
    }

    /// <summary>
    /// Получить соседних тяжёлых воинов с учётом боевого построения
    /// </summary>
    public List<IUnit> GetAvailableTargets(IArmy friendlyArmy, IBattleFormation? formation = null)
    {
        var availableTargets = new List<IUnit>();

        if (_unitPosition == 0 || _squire == null)
            return availableTargets;

        // Если есть formation и это 3 на 3, используем соседей по сетке
        if (formation is ThreeOnThreeFormation)
        {
            // Получаем соседей через formation
            var adjacentUnits = formation.GetAdjacentUnits(_squire, friendlyArmy);
            // Фильтруем - нужны только Buffable юниты
            return adjacentUnits.Where(u => u is IBuffable && u.IsAlive).ToList();
        }

        // Для режима 1 на 1 (Bridge) или если formation не задана: старая логика
        // Ищем тяжёлых воинов спереди и сзади
        int frontPosition = _unitPosition - 1;
        int backPosition = _unitPosition + 1;

        // Проверяем спереди
        if (frontPosition >= 0 && frontPosition < friendlyArmy.Units.Count)
        {
            var frontUnit = friendlyArmy.Units[frontPosition];
            if (frontUnit is IBuffable && frontUnit.IsAlive)
            {
                availableTargets.Add(frontUnit);
            }
        }

        // Проверяем сзади
        if (backPosition >= 0 && backPosition < friendlyArmy.Units.Count)
        {
            var backUnit = friendlyArmy.Units[backPosition];
            if (backUnit is IBuffable && backUnit.IsAlive)
            {
                availableTargets.Add(backUnit);
            }
        }

        return availableTargets;
    }
}
