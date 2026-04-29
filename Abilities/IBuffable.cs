using System.Collections.Generic;

/// <summary>
/// Интерфейс для юнитов, которые могут получать баффы
/// </summary>
public interface IBuffable : IUnit
{
    /// <summary>
    /// Добавить бафф к юниту
    /// </summary>
    void AddBuff(IBuff buff);

    /// <summary>
    /// Получить все активные баффы
    /// </summary>
    List<IBuff> GetActiveBuffs();

    /// <summary>
    /// Удалить случайный бафф
    /// </summary>
    bool RemoveRandomBuff();

    /// <summary>
    /// Есть ли баффы
    /// </summary>
    bool HasBuffs();
}
