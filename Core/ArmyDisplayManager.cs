using System;
using System.Linq;

/// <summary>
/// Управление отображением информации об армиях
/// Отделена от логики игры
/// </summary>
public class ArmyDisplayManager
{
    private ArmyBuilder _armyBuilder;

    public ArmyDisplayManager()
    {
        _armyBuilder = new ArmyBuilder();
    }

    /// <summary>
    /// Показать информацию об армии одного игрока
    /// </summary>
    public void ShowArmyInfo(Player player)
    {
        if (player.Army == null)
        {
            Console.WriteLine("Армия не создана");
            return;
        }

        Console.WriteLine(new string('=', 63));
        Console.WriteLine($"АРМИЯ {player.Name?.ToUpper()}");
        Console.WriteLine(new string('=', 63));

        Console.WriteLine($"Общая стоимость: {player.Army.Units.Sum(u => u.Cost)}");
        Console.WriteLine($"Количество юнитов: {player.Army.Units.Count}\n");

        _armyBuilder.PrintUnitTable(player.Army.Units);
    }

    /// <summary>
    /// Показать информацию об обеих армиях
    /// </summary>
    public void DisplayArmies(Player player1, Player player2)
    {
        Console.WriteLine(new string('=', 63));
        Console.WriteLine($"АРМИЯ {player1.Name?.ToUpper()}");
        Console.WriteLine(new string('=', 63));

        if (player1.Army != null)
        {
            Console.WriteLine($"Общая стоимость: {player1.Army.Units.Sum(u => u.Cost)}");
            Console.WriteLine($"Количество юнитов: {player1.Army.Units.Count}\n");

            _armyBuilder.PrintUnitTable(player1.Army.Units);
        }

        Console.WriteLine("\n" + new string('=', 63));
        Console.WriteLine($"АРМИЯ {player2.Name?.ToUpper()}");
        Console.WriteLine(new string('=', 63));

        if (player2.Army != null)
        {
            Console.WriteLine($"Общая стоимость: {player2.Army.Units.Sum(u => u.Cost)}");
            Console.WriteLine($"Количество юнитов: {player2.Army.Units.Count}\n");

            _armyBuilder.PrintUnitTable(player2.Army.Units);
        }

        Console.WriteLine(new string('=', 63) + "\n");
    }
}
