/// <summary>
/// Бафф Шлем - добавляет защиту от стрел (уменьшает урон от стрел на 2)
/// </summary>
public class HelmetBuff : IBuff
{
    private int _arrowDefenseBonus;

    public string Name => "Helmet";
    public int AttackModifier => 0;
    public int DefenseModifier => 0;
    public int ArrowDefenseModifier => _arrowDefenseBonus;

    public HelmetBuff(int arrowDefenseBonus = 2)
    {
        _arrowDefenseBonus = arrowDefenseBonus;
    }
}
