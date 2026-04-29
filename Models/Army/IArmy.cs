using System.Collections.Generic;

/// <summary>
/// Контракт для управления армией боевых юнитов
/// </summary>
public interface IArmy
{
    /// <summary>
    /// Название армии
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Список юнитов в армии
    /// </summary>
    List<IUnit> Units { get; }

    /// <summary>
    /// Добавить юнита в армию
    /// </summary>
    void AddUnit(IUnit unit);

    /// <summary>
    /// Удалить юнита из армии
    /// </summary>
    void RemoveUnit(IUnit unit);

    /// <summary>
    /// Проверить, есть ли живые юниты в армии
    /// </summary>
    bool HasAliveUnits();

    /// <summary>
    /// Получить текущего живого юнита (первый в списке)
    /// </summary>
    IUnit? GetCurrentUnit();

    /// <summary>
    /// Удалить мёртвых юнитов из армии
    /// </summary>
    void RemoveDeadUnits();
}
