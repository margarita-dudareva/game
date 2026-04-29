/// <summary>
/// Бафф Щит - добавляет защиту от удара
/// </summary>
public class ShieldBuff : IBuff
{
    private int _defenseBonus;

    public string Name => "Shield";
    public int AttackModifier => 0;
    public int DefenseModifier => _defenseBonus;
    public int ArrowDefenseModifier => 0;

    public ShieldBuff(int defenseBonus)
    {
        _defenseBonus = defenseBonus;
    }
}
