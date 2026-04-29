/// <summary>
/// Бафф Копьё - добавляет силу атаки
/// </summary>
public class SpearBuff : IBuff
{
    private int _attackBonus;

    public string Name => "Spear";
    public int AttackModifier => _attackBonus;
    public int DefenseModifier => 0;
    public int ArrowDefenseModifier => 0;

    public SpearBuff(int attackBonus)
    {
        _attackBonus = attackBonus;
    }
}
