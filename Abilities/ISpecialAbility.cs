/// <summary>
/// Контракт для особых способностей
/// </summary>
public interface ISpecialAbility
{
    /// <summary>
    /// Проверяет, можно ли применить special ability
    /// </summary>
    bool CanActivate(IUnit target);

    /// <summary>
    /// Применяет special ability к мишени
    /// </summary>
    void Activate(IUnit target);

    /// <summary>
    /// Название способности
    /// </summary>
    string Name { get; }
}
