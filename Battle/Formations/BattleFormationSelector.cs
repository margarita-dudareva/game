using System;
using System.Collections.Generic;

/// <summary>
/// Меню для выбора боевого построения перед началом боя
/// </summary>
public class BattleFormationSelector
{
    private List<IBattleFormation> _formations;

    public BattleFormationSelector()
    {
        _formations = new List<IBattleFormation>
        {
            new BridgeFormation(),
            new ThreeOnThreeFormation(),
            new WallToWallFormation()
        };
    }

    /// <summary>
    /// Показать меню выбора и получить выбранное построение
    /// </summary>
    public IBattleFormation SelectFormation()
    {
        Console.WriteLine("\n" + new string('=', 50));
        Console.WriteLine("Выберите боевое построение:");
        Console.WriteLine(new string('=', 50));

        for (int i = 0; i < _formations.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {_formations[i].Name}");
        }

        int choice;
        while (true)
        {
            Console.Write("\nВаш выбор (введите номер): ");
            if (int.TryParse(Console.ReadLine(), out choice) && choice > 0 && choice <= _formations.Count)
            {
                break;
            }
            Console.WriteLine("Некорректный ввод. Попробуйте снова.");
        }

        return _formations[choice - 1];
    }

    /// <summary>
    /// Получить построение по умолчанию (боевой мост)
    /// </summary>
    public IBattleFormation GetDefaultFormation()
    {
        return _formations[0];
    }
}
