using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Класс для создания армий (случайно или вручную)
/// Работает с фабриками для создания юнитов
/// </summary>
public class ArmyBuilder
{
    private readonly IUnitFactoryProvider _unitFactoryProvider;

    public ArmyBuilder(IUnitFactoryProvider? unitFactoryProvider = null)
    {
        _unitFactoryProvider = unitFactoryProvider ?? new UnitFactoryProvider();
    }

    /// <summary>
    /// Универсальный метод для создания армии с использованием фабрик
    /// Позволяет добавлять новые типы юнитов без изменения этого метода
    /// </summary>
    /// <param name="name">Название армии</param>
    /// <param name="budget">Бюджет армии</param>
    /// <param name="unitFactories">Список фабрик для создания юнитов</param>
    public Army CreateRandomArmyWithFactories(string name, int budget, List<IUnitFactory> unitFactories)
    {
        if (unitFactories == null || unitFactories.Count == 0)
            throw new ArgumentException("Должна быть хотя бы одна фабрика юнитов!");

        var army = new Army(name);
        var random = new Random();
        int remainingBudget = budget;

        // НЕ используем опции напрямую - создаём новые юниты для армии
        var unitPool = GenerateUnitOptions();

        while (unitPool.Count > 0 && remainingBudget > 0)
        {
            int randomIndex = SelectWeightedByAttack(unitPool, random);
            IUnit unit = unitPool[randomIndex];

            if (remainingBudget >= unit.Cost)
            {
                // Создать новый юнит с правильным ID вместо использования опции напрямую
                var newUnit = DuplicateUnitWithNewId(unit);
                army.AddUnit(newUnit);
                remainingBudget -= unit.Cost;
            }
            
            // Удаляем юнита из пула независимо от того, добавили мы его или нет
            unitPool.RemoveAt(randomIndex);
        }

        return army;
    }

    /// <summary>
    /// <summary>
    /// Выбрать юнита из списка с приоритетом по атаке (более сильные выбираются чаще)
    /// Каждый юнит имеет минимальный вес 1, чтобы гарантировано выбраться
    /// </summary>
    private int SelectWeightedByAttack(List<IUnit> units, Random random)
    {
        if (units.Count == 1)
            return 0;

        // Каждому юниту минимальный вес 1, плюс его атака
        // Так даже юниты с attack=0 имеют шанс быть выбранными
        int totalWeight = units.Sum(u => Math.Max(1, u.Attack));

        int randomValue = random.Next(totalWeight);

        int currentSum = 0;
        for (int i = 0; i < units.Count; i++)
        {
            int unitWeight = Math.Max(1, units[i].Attack);
            currentSum += unitWeight;
            if (randomValue < currentSum)
                return i;
        }

        return units.Count - 1;
    }

    /// <summary>
    /// Получить стандартный набор фабрик (Light Infantry, Heavy Infantry, Archer, Healer, Wizard)
    /// </summary>
    public List<IUnitFactory> GetDefaultUnitFactories()
    {
        return _unitFactoryProvider.GetUnitFactories();
    }

    /// <summary>
    /// Создать список доступных юнитов для ручного выбора
    /// Использует фабрики для создания юнитов
    /// </summary>
    public List<IUnit> GenerateUnitOptions()
    {
        var list = new List<IUnit>();
        var factories = GetDefaultUnitFactories();

        foreach (var factory in factories)
        {
            for (int i = 0; i < 4; i++)
            {
                list.Add(factory.CreateUnit());
            }
        }

        return list;
    }

    /// <summary>
    /// Создать копию юнита с новым ID (для добавления в армию)
    /// </summary>
    public IUnit DuplicateUnitWithNewId(IUnit template)
    {
        IUnit? newUnit = null;

        // Определить тип через SpecialAbility (работает даже с прокси)
        if (template.SpecialAbility is ArrowAbility arrowAbility)
        {
            newUnit = new UnitBase(template.Name, template.Attack, template.Defense, 
                                 template.MaxHealth, template.Cost, 
                                 new ArrowAbility(arrowAbility.Range, arrowAbility.Power));
        }
        else if (template.SpecialAbility is HealAbility healAbility)
        {
            // Использовать параметры из шаблона для соответствия стоимости
            newUnit = new Healer(template.Attack, template.Defense, template.MaxHealth, template.Cost,
                                healAbility.Range, healAbility.HealPower);
        }
        else if (template.SpecialAbility is CloneAbility cloneAbility)
        {
            // Использовать параметры из шаблона для соответствия стоимости
            newUnit = new Wizard(template.Attack, template.Defense, template.MaxHealth, template.Cost,
                                cloneAbility.Range, cloneAbility.CloneProbability);
        }
        else
        {
            // Для пехоты проверяем тип напрямую
            // Сначала проверяем через имя класса (включая базовый класс через рефлексию)
            var type = template.GetType();
            
            // Если это прокси, получаем базовый тип
            if (type.Name.Contains("Proxy") && type.BaseType != null)
            {
                type = type.BaseType;
            }
            
            if (type.Name.Contains("LightInfantry") || template.Name == "Light Infantry")
            {
                newUnit = new LightInfantry(template.Attack, template.Defense, template.MaxHealth, template.Cost);
            }
            else if (type.Name.Contains("HeavyInfantry") || template.Name == "Heavy Infantry")
            {
                newUnit = new BuffableHeavyInfantry(template.Attack, template.Defense, template.MaxHealth, template.Cost);
            }
            else
            {
                // Если вообще не опознали, вернуть исходный (на худой конец)
                newUnit = template;
            }
        }

        // Подписать наблюдателей (логирование и бип) если они включены
        return ApplyObservers(newUnit ?? template);
    }

    /// <summary>
    /// Применить наблюдателей к юниту согласно настройкам
    /// </summary>
    private IUnit ApplyObservers(IUnit unit)
    {
        if (ProxySettings.Current.EnableDamageLogging)
        {
            unit.Subscribe(new DamageLoggerObserver());
        }

        if (ProxySettings.Current.EnableDeathBeep)
        {
            unit.Subscribe(new DeathBeepObserver());
        }

        return unit;
    }

    /// <summary>
    /// Вывести таблицу юнитов на консоль
    /// </summary>
    public void PrintUnitTable(List<IUnit> units, bool includeArcherFields = true)
    {
        Console.WriteLine(new string('=', 125));
        Console.WriteLine(includeArcherFields
            ? "№  Имя             Атака  Защита  Здоровье  Стоимость  Дальность  Сила Стрелы  Сила Лечения  Вер. Клона"
            : "№  Имя             Атака  Защита  Здоровье  Стоимость");
        Console.WriteLine(new string('-', 124));

        for (int i = 0; i < units.Count; i++)
        {
            var u = units[i];

            if (includeArcherFields && u.SpecialAbility is ArrowAbility arrowAbility)
            {
                Console.WriteLine($"{i + 1,-3}{u.Name,-16}{u.Attack,-7}{u.Defense,-8}{u.MaxHealth,-10}{u.Cost,-11}{arrowAbility.Range,-11}{arrowAbility.Power,-12}{"-",-13}");
            }
            else if (includeArcherFields && u.SpecialAbility is HealAbility healAbility)
            {
                Console.WriteLine($"{i + 1,-3}{u.Name,-16}{u.Attack,-7}{u.Defense,-8}{u.MaxHealth,-10}{u.Cost,-11}{healAbility.Range,-11}{"-",-12}{healAbility.HealPower,-13}");
            }
            else if (includeArcherFields && u.SpecialAbility is CloneAbility cloneAbility)
            {
                Console.WriteLine($"{i + 1,-3}{u.Name,-16}{u.Attack,-7}{u.Defense,-8}{u.MaxHealth,-10}{u.Cost,-11}{cloneAbility.Range,-11}{"-",-12}{"-",-13}{cloneAbility.CloneProbability}%");
            }
            else
            {
                Console.WriteLine($"{i + 1,-3}{u.Name,-16}{u.Attack,-7}{u.Defense,-8}{u.MaxHealth,-10}{u.Cost}");
            }
        }

        Console.WriteLine(new string('=', 125));
    }
}

