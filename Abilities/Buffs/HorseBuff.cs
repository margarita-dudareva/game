/// <summary>
/// Бафф Конь - добавляет и атаку и защиту
/// </summary>
public class HorseBuff : IBuff
{
    private int _bonus;

    public string Name => "Horse";
    public int AttackModifier => _bonus;
    public int DefenseModifier => _bonus;
    public int ArrowDefenseModifier => 0;

    public HorseBuff(int bonus)
    {
        _bonus = bonus;
    }
}
