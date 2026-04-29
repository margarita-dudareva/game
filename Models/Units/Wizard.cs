/// <summary>
/// Волшебник - юнит с способностью клонирования
/// Может атаковать как обычный юнит и клонировать пехоту/лучников
/// </summary>
public class Wizard : UnitBase
{
    private static Random _random = new Random();

    public Wizard() : base("Wizard", GenerateAttack(), GenerateDefense(), GenerateHealth(), GenerateCost(),
                          new CloneAbility(GenerateRange(), GenerateCloneProbability()))
    {
    }

    private static int GenerateAttack() => _random.Next(15, 22);
    private static int GenerateDefense() => _random.Next(4, 9);
    private static int GenerateHealth() => _random.Next(20, 28);
    private static int GenerateCost() => _random.Next(50, 70);
    private static int GenerateRange() => _random.Next(1, 3);
    private static int GenerateCloneProbability() => _random.Next(30, 45); // 30-45% вероятность успеха
}
