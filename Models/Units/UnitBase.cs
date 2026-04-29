using Game.Models.Units.Adapters;
using System.Collections.Generic;

public class UnitBase : IUnit
{
    private readonly ISpecialAbility? _specialAbility;
    private readonly List<IUnitEventObserver> _observers = new List<IUnitEventObserver>();

    public int Id { get; private set; } = 0;
    public string Name { get; }
    public int Attack { get; }
    public int Defense { get; }
    public int MaxHealth { get; }
    public int CurrentHealth { get; private set; }
    public int Cost { get; }
    public virtual ISpecialAbility? SpecialAbility => _specialAbility;

    public bool IsAlive => CurrentHealth > 0;

    public UnitBase(string name, int attack, int defense, int health, int cost, ISpecialAbility? specialAbility = null)
    {
        Name = name;
        Attack = attack;
        Defense = defense;
        MaxHealth = health;
        CurrentHealth = health;
        Cost = cost;
        _specialAbility = specialAbility;
    }

    /// <summary>
    /// Установить ID юнита (используется при присвоении номера и восстановлении из сохраненной игры)
    /// </summary>
    public void SetId(int id)
    {
        Id = id;
    }



    public virtual int CalculateDamage()
    {
        return Attack;
    }

    public virtual void TakeDamage(int incomingDamage)
    {
        bool wasAlive = IsAlive;
        int healthBefore = CurrentHealth;
        CurrentHealth -= incomingDamage;

        if (CurrentHealth < 0)
            CurrentHealth = 0;

        int healthAfter = CurrentHealth;
        int damageTaken = healthBefore - healthAfter;

        // Уведомляем наблюдателей о получении урона
        NotifyDamageTaken(new DamageTakenEventArgs(this, damageTaken, healthBefore, healthAfter));

        // Уведомляем наблюдателей о смерти (если только что произошла)
        if (wasAlive && !IsAlive)
        {
            NotifyUnitDeath(new UnitDeathEventArgs(this));
        }
    }

    /// <summary>
    /// Восстановить HP юнита (для лечения)
    /// </summary>
    public virtual void Heal(int healAmount)
    {
        int newHealth = CurrentHealth + healAmount;
        CurrentHealth = Math.Min(newHealth, MaxHealth);
    }

    /// <summary>
    /// Установить текущее здоровье (для клонирования)
    /// </summary>
    public virtual void SetCurrentHealth(int newHealth)
    {
        CurrentHealth = Math.Max(0, Math.Min(newHealth, MaxHealth));
    }

    /// <summary>
    /// Получить символ для визуализации армии
    /// </summary>
    public virtual char GetDisplaySymbol()
    {
        if (SpecialAbility is ArrowAbility)
            return 'A';
        if (SpecialAbility is CloneAbility)
            return 'W';
        if (SpecialAbility is HealAbility)
            return 'H';
        if (this is HeavyInfantry)
            return 'V';
        if (this is LightInfantry)
            return 'L';
        if (this is GulyayGorodAdapter)
            return 'G';
        return '?';
    }

    /// <summary>
    /// Получить короткое имя юнита для логирования  
    /// </summary>
    public virtual string GetShortDisplayName()
    {
        if (SpecialAbility is ArrowAbility)
            return "Archer";
        if (SpecialAbility is CloneAbility)
            return "Wizard";
        if (SpecialAbility is HealAbility)
            return "Healer";
        if (this is HeavyInfantry)
            return "Heavy Inf.";
        if (this is LightInfantry)
            return "Light Inf.";
        if (this is GulyayGorodAdapter)
            return "GulyayGorod";
        return this.Name;
    }

    /// <summary>
    /// Подписать наблюдателя на события юнита
    /// </summary>
    public void Subscribe(IUnitEventObserver observer)
    {
        if (observer != null && !_observers.Contains(observer))
        {
            _observers.Add(observer);
        }
    }

    /// <summary>
    /// Отписать наблюдателя от событий юнита
    /// </summary>
    public void Unsubscribe(IUnitEventObserver observer)
    {
        if (observer != null)
        {
            _observers.Remove(observer);
        }
    }

    /// <summary>
    /// Уведомить наблюдателей о получении урона
    /// </summary>
    private void NotifyDamageTaken(DamageTakenEventArgs args)
    {
        foreach (var observer in _observers)
        {
            observer.OnDamageTaken(args);
        }
    }

    /// <summary>
    /// Уведомить наблюдателей о смерти
    /// </summary>
    private void NotifyUnitDeath(UnitDeathEventArgs args)
    {
        foreach (var observer in _observers)
        {
            observer.OnUnitDeath(args);
        }
    }
}
