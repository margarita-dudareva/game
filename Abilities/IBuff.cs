/// <summary>
/// Интерфейс для баффов (использует паттерн Декоратор)
/// </summary>
public interface IBuff
{
    /// <summary>
    /// Название баффа
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Модификация атаки
    /// </summary>
    int AttackModifier { get; }

    /// <summary>
    /// Модификация защиты
    /// </summary>
    int DefenseModifier { get; }

    /// <summary>
    /// Модификация защиты от стрел (урон стрел уменьшается на это значение)
    /// </summary>
    int ArrowDefenseModifier { get; }
}
