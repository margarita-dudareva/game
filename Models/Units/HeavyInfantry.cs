public class HeavyInfantry : UnitBase
{
    private static Random _random = new Random();

    public HeavyInfantry() : base("Heavy Infantry", GenerateAttack(), GenerateDefense(), GenerateHealth(), GenerateCost())
    {
    }

    /// <summary>
    /// Конструктор для создания копии с заданными параметрами (для клонирования)
    /// </summary>
    public HeavyInfantry(int attack, int defense, int health, int cost) 
        : base("Heavy Infantry", attack, defense, health, cost)
    {
    }

    private static int GenerateAttack() => _random.Next(35, 40); 
    private static int GenerateDefense() => _random.Next(10, 15);
    private static int GenerateHealth() => _random.Next(34, 46);     
    private static int GenerateCost() => _random.Next(45, 66);    
}
