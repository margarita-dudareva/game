using Game.Models.Units.Adapters;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// HeavyInfantry с поддержкой баффов (реализует паттерн Декоратор через баффы)
/// </summary>
public class BuffableHeavyInfantry : IBuffable
{
    private readonly ISpecialAbility? _specialAbility;
    private readonly List<IBuff> _activeBuffs;
    private readonly List<IUnitEventObserver> _observers = new List<IUnitEventObserver>();
    private readonly int _baseAttack;
    private readonly int _baseDefense;
    private static Random _random = new Random();

    public int Id { get; private set; } = 0;
    public string Name { get; }
    public int MaxHealth { get; }
    public int CurrentHealth { get; private set; }
    public int Cost { get; }
    public virtual ISpecialAbility? SpecialAbility => _specialAbility;
    public bool IsAlive => CurrentHealth > 0;

    /// <summary>
    /// Атака с учётом баффов
    /// </summary>
    public int Attack 
    { 
        get 
        { 
            int totalModifier = _activeBuffs.Sum(b => b.AttackModifier);
            return _baseAttack + totalModifier;
        } 
    }

    /// <summary>
    /// Защита с учётом баффов
    /// </summary>
    public int Defense 
    { 
        get 
        { 
            int totalModifier = _activeBuffs.Sum(b => b.DefenseModifier);
            return _baseDefense + totalModifier;
        } 
    }

    public BuffableHeavyInfantry() 
        : this(GenerateAttack(), GenerateDefense(), GenerateHealth(), GenerateCost())
    {
    }

    /// <summary>
    /// Конструктор для создания копии с заданными параметрами (для клонирования)
    /// </summary>
    public BuffableHeavyInfantry(int attack, int defense, int health, int cost) 
    {
        Name = "Heavy Infantry";
        _baseAttack = attack;
        _baseDefense = defense;
        MaxHealth = health;
        CurrentHealth = health;
        Cost = cost;
        _specialAbility = null;
        _activeBuffs = new List<IBuff>();
    }

    private static int GenerateAttack() => _random.Next(35, 40);
    private static int GenerateDefense() => _random.Next(10, 15);
    private static int GenerateHealth() => _random.Next(34, 46);
    private static int GenerateCost() => _random.Next(45, 66);

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

    /// <summary>
    /// Получить урон, учитывая защиту от стрел
    /// </summary>
    public int CalculateDamageWithArrowDefense(int incomingDamage)
    {
        int arrowDefenseBonus = _activeBuffs.Sum(b => b.ArrowDefenseModifier);
        return Math.Max(0, incomingDamage - arrowDefenseBonus);
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
        CurrentHealth = System.Math.Min(newHealth, MaxHealth);
    }

    /// <summary>
    /// Установить текущее здоровье (для клонирования)
    /// </summary>
    public virtual void SetCurrentHealth(int newHealth)
    {
        CurrentHealth = System.Math.Max(0, System.Math.Min(newHealth, MaxHealth));
    }

    /// <summary>
    /// Получить символ для визуализации армии
    /// </summary>
    public virtual char GetDisplaySymbol()
    {
        return 'V'; // Heavy Infantry
    }

    /// <summary>
    /// Получить короткое имя юнита для логирования
    /// </summary>
    public virtual string GetShortDisplayName()
    {
        return $"V{Id}";
    }

    // IBuffable реализация
    
    /// <summary>
    /// Добавить бафф к юниту
    /// </summary>
    public void AddBuff(IBuff buff)
    {
        _activeBuffs.Add(buff);
    }

    /// <summary>
    /// Получить все активные баффы
    /// </summary>
    public List<IBuff> GetActiveBuffs()
    {
        return new List<IBuff>(_activeBuffs);
    }

    /// <summary>
    /// Удалить случайный бафф
    /// </summary>
    public bool RemoveRandomBuff()
    {
        if (_activeBuffs.Count == 0)
            return false;

        int randomIndex = _random.Next(_activeBuffs.Count);
        _activeBuffs.RemoveAt(randomIndex);
        return true;
    }

    /// <summary>
    /// Есть ли баффы
    /// </summary>
    public bool HasBuffs()
    {
        return _activeBuffs.Count > 0;
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
