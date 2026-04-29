using Game.Models.Units.Adapters;

/// <summary>
/// Фабрика для создания обычных легких пехотинцев
/// </summary>
public class LightInfantryFactory : IUnitFactory
{
    public string UnitTypeName => "Light Infantry";
    
    public IUnit CreateUnit()
    {
        return new LightInfantry();
    }
}

/// <summary>
/// Фабрика для создания тяжелых пехотинцев с поддержкой баффов
/// </summary>
public class HeavyInfantryFactory : IUnitFactory
{
    public string UnitTypeName => "Heavy Infantry";
    
    public IUnit CreateUnit()
    {
        return new BuffableHeavyInfantry();
    }
}

/// <summary>
/// Фабрика для создания лучников (UnitBase с луком)
/// </summary>
public class ArcherFactory : IUnitFactory
{
    public string UnitTypeName => "Archer";
    private static Random _random = new Random();

    public IUnit CreateUnit()
    {
        int range = _random.Next(2, 5);
        int power = _random.Next(20, 25);
        return new UnitBase("Archer", GenerateAttack(), GenerateDefense(), GenerateHealth(), GenerateCost(), new ArrowAbility(range, power));
    }

    private static int GenerateAttack() => _random.Next(20, 25);
    private static int GenerateDefense() => _random.Next(2, 7);
    private static int GenerateHealth() => _random.Next(20, 30);
    private static int GenerateCost() => _random.Next(20, 36);
}

/// <summary>
/// Фабрика для создания лекарей (Healer с HealAbility)
/// </summary>
public class HealerFactory : IUnitFactory
{
    public string UnitTypeName => "Healer";

    public IUnit CreateUnit()
    {
        return new Healer();
    }
}

/// <summary>
/// Фабрика для создания волшебников (Wizard с CloneAbility)
/// </summary>
public class WizardFactory : IUnitFactory
{
    public string UnitTypeName => "Wizard";

    public IUnit CreateUnit()
    {
        return new Wizard();
    }
}

/// <summary>
/// Фабрика для создания Гуляй-города (крепость из Adapter Pattern)
/// </summary>
public class GulyayGorodAdapterFactory : IUnitFactory
{
    public string UnitTypeName => "Gulyay-Gorod";

    public IUnit CreateUnit()
    {
        return new GulyayGorodAdapter();
    }
}
