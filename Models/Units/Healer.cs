/// <summary>
/// Лекарь - юнит, который может лечить других союзников
/// Может атаковать (когда стоит первым), но основная его функция - лечение
/// </summary>
public class Healer : UnitBase, ICanBeHealed
{
    private static Random _random = new Random();

    public Healer() : base("Healer", GenerateAttack(), GenerateDefense(), GenerateHealth(), GenerateCost(), 
                           new HealAbility(GenerateRange(), GenerateHealPower()))
    {
    }

    private static int GenerateAttack() => _random.Next(8, 15);
    private static int GenerateDefense() => _random.Next(3, 8);
    private static int GenerateHealth() => _random.Next(18, 25);
    private static int GenerateCost() => _random.Next(40, 55);
    private static int GenerateRange() => _random.Next(1, 3);
    private static int GenerateHealPower() => _random.Next(8, 15);
}
