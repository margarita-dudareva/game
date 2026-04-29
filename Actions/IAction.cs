/// <summary>
/// Интерфейс для любого боевого действия (Command pattern)
/// Расширяемо - можно добавлять DefenseAction, HealAction и т.д.
/// </summary>
public interface IAction
{
    string Name { get; }
    void Execute(IUnit attacker, IUnit target);
}
