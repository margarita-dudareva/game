/// <summary>
/// Класс LightInfantry - оруженосец с поддержкой лечения и способностью применять баффы
/// </summary>
public class LightInfantry : UnitBase, ICanBeHealed
{
    private static Random _random = new Random();

    public LightInfantry() : base("Light Infantry", GenerateAttack(), GenerateDefense(), GenerateHealth(), GenerateCost(), new SquireAbility())
    {
    }

    /// <summary>
    /// Конструктор для создания копии с заданными параметрами (для клонирования)
    /// </summary>
    public LightInfantry(int attack, int defense, int health, int cost) 
        : base("Light Infantry", attack, defense, health, cost, new SquireAbility())
    {
    }

    private static int GenerateAttack() => _random.Next(20, 25);
    private static int GenerateDefense() => _random.Next(2, 7);
    private static int GenerateHealth() => _random.Next(20, 30);
    private static int GenerateCost() => _random.Next(20, 36);
}
