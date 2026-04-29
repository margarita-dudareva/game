/// <summary>
/// Представление игрока с именем и армией
/// </summary>
public class Player
{
    public string Name { get; }
    public IArmy? Army { get; set; }

    public Player(string name)
    {
        Name = name;
    }
}

