public interface IUnit
{
    int Id { get; }
    string Name { get; }
    int Attack { get; }
    int Defense { get; }
    int MaxHealth { get; }
    int CurrentHealth { get; }
    int Cost { get; }
    bool IsAlive { get; }
    ISpecialAbility? SpecialAbility { get; }

    int CalculateDamage(); 
    void TakeDamage(int incomingDamage);
    void Heal(int healAmount);
    void SetCurrentHealth(int newHealth);
    void SetId(int id);
    
    /// <summary>
    /// Получить символ для визуализации армии
    /// </summary>
    char GetDisplaySymbol();

    /// <summary>
    /// Получить короткое имя юнита для логирования
    /// </summary>
    string GetShortDisplayName();

    /// <summary>
    /// Подписать наблюдателя на события юнита
    /// </summary>
    void Subscribe(IUnitEventObserver observer);

    /// <summary>
    /// Отписать наблюдателя от событий юнита
    /// </summary>
    void Unsubscribe(IUnitEventObserver observer);
}


