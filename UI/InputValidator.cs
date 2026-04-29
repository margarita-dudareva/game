using System;

/// <summary>
/// Валидация и получение пользовательского ввода
/// </summary>
public class InputValidator
{
    // Стандартный бюджет для создания армии
    public const int DEFAULT_BUDGET = 500;

    public string GetPlayerName(int playerNumber)
    {
        Console.WriteLine($"Введите имя {(playerNumber == 1 ? "первого" : "второго")} игрока:");
        var input = Console.ReadLine() ?? "";
        return string.IsNullOrWhiteSpace(input) ? $"Player {playerNumber}" : input;
    }

    public int GetBudget()
    {
        Console.WriteLine($"\nВведите бюджет армии (по умолчанию {DEFAULT_BUDGET}):");
        Console.WriteLine($"Нажмите Enter, чтобы оставить {DEFAULT_BUDGET}");

        var input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
            return DEFAULT_BUDGET;

        return int.TryParse(input, out int result) ? result : DEFAULT_BUDGET;
    }

    public int GetArmyCreationMode()
    {
        var input = Console.ReadLine() ?? "";
        return int.TryParse(input, out int choice) ? choice : -1;
    }

    public int GetManualArmyChoice(int maxOptions)
    {
        var input = Console.ReadLine();

        if (!int.TryParse(input, out int choice))
            return -1;

        return choice;
    }

    public bool GetYesNo(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = (Console.ReadLine() ?? string.Empty).Trim().ToUpperInvariant();

            if (input == "Y" || input == "YES" || input == "Д" || input == "ДА")
                return true;

            if (input == "N" || input == "NO" || input == "Н" || input == "НЕТ")
                return false;

            Console.WriteLine("Пожалуйста, введите Y/N (или Д/Н).\n");
        }
    }
}
