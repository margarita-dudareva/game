public class AttackAction : IAction 
{
    public string Name => "Attack";

    /// <summary>
    /// Атака с учетом защиты цели
    /// </summary> 
    public void Execute(IUnit attacker, IUnit target)
    {
        int rawDamage = attacker.CalculateDamage();
        int actualDamage = Math.Max(0, rawDamage - target.Defense);
        int healthBefore = target.CurrentHealth;
        
        Console.WriteLine($"{attacker.Name} атакует {target.Name}! Урон: {rawDamage} (Защита: {target.Defense})");
        
        target.TakeDamage(actualDamage);
        
        int healthAfter = target.CurrentHealth;
        int damageDealt = healthBefore - healthAfter;
        
        Console.WriteLine($"{target.Name} получил {damageDealt} урона. Здоровье: {healthAfter}/{target.MaxHealth}");
    }

    /// <summary>
    /// Атака с глобальными номерами юнитов
    /// </summary>
    public void Execute(IUnit attacker, IUnit target, int attackerGlobalNum, int targetGlobalNum)
    {
        int rawDamage = attacker.CalculateDamage();
        int actualDamage = Math.Max(0, rawDamage - target.Defense);
        int healthBefore = target.CurrentHealth;
        
        Console.WriteLine($"#{attackerGlobalNum} {attacker.Name} атакует #{targetGlobalNum} {target.Name}! Урон: {rawDamage} (Защита: {target.Defense})");
        
        target.TakeDamage(actualDamage);
        
        int healthAfter = target.CurrentHealth;
        int damageDealt = healthBefore - healthAfter;
        
        Console.WriteLine($"#{targetGlobalNum} {target.Name} получил {damageDealt} урона. Здоровье: {healthAfter}/{target.MaxHealth}");
    }
}