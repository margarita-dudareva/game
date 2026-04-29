using System;
using System.Collections.Generic;

/// <summary>
/// Стратегия для управления боевым построением армии
/// Определяет, кто находится на ринге (сражается), кто в запасе (спецсособности)
/// и как обрабатываются замены при смерти юнитов
/// </summary>
public interface IBattleFormation
{
    /// <summary>
    /// Название построения
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Получить юнитов, находящихся "на ринге" (участвующих в боевых действиях)
    /// </summary>
    List<IUnit> GetRingUnits(IArmy army);

    /// <summary>
    /// Получить юнитов в "запасе" (не на ринге, могут использовать спецсособности)
    /// </summary>
    List<IUnit> GetReserveUnits(IArmy army);

    /// <summary>
    /// Проверить, находится ли юнит на ринге
    /// </summary>
    bool IsOnRing(IUnit unit, IArmy army);

    /// <summary>
    /// Получить противника для конкретного юнита (его боевую пару на ринге)
    /// </summary>
    IUnit? GetOpponent(IUnit unit, IArmy army, IArmy opponentArmy);

    /// <summary>
    /// Получить соседних юнитов (для спецсособностей типа "рядом")
    /// Для Bridge: пусто, для остальных: все окружающие включая диагонали
    /// </summary>
    List<IUnit> GetAdjacentUnits(IUnit unit, IArmy army);

    /// <summary>
    /// Обработать смерть юнита: замены, перестройка армии
    /// </summary>
    void HandleUnitDeath(IUnit deadUnit, IArmy army, IArmy opponentArmy);

    /// <summary>
    /// Переместить юнита на ринг (поднять из запаса)
    /// </summary>
    void PromoteUnitToRing(IUnit unit, IArmy army);

    /// <summary>
    /// Получить позицию юнита в построении (для визуализации и логики)
    /// </summary>
    (int row, int column) GetUnitPosition(IUnit unit, IArmy army);

    /// <summary>
    /// Получить юнитов в радиусе (для спецсособностей с дальностью)
    /// </summary>
    List<IUnit> GetUnitsInRadius(IUnit unit, IArmy army, int radius);
}
