using System;

/// <summary>
/// Управление главным меню игры
/// Отвечает за отображение и обработку выбора пользователя
/// </summary>
public class MenuManager
{
    public void ShowMainMenu()
    {
        try
        {
            Console.Clear();
        }
        catch
        {
            // Если нет доступа к консоли (например, при пайпинге), просто пропускаем
        }
        
        Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║              МЕХАНИКА БОЕВОЙ СИСТЕМЫ                       ║");
        Console.WriteLine("╠════════════════════════════════════════════════════════════╣");
        Console.WriteLine("║ 1. Начать новую игру                                       ║");
        Console.WriteLine("║ 2. Загрузить сохраненную игру                              ║");
        Console.WriteLine("║ 3. Настройки прокси                                        ║");
        Console.WriteLine("║ 4. Выход                                                   ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
    }

    public int GetMenuChoice()
    {
        Console.Write("\nВыберите: ");
        string input = Console.ReadLine() ?? "";
        
        if (int.TryParse(input, out int choice))
            return choice;
        
        return -1;
    }

    public void ShowInvalidChoiceMessage()
    {
        Console.WriteLine("Неверный выбор!");
        Console.ReadKey();
    }

    public void ShowBudgetPrompt()
    {
        Console.WriteLine($"\nВведите бюджет армии (по умолчанию {InputValidator.DEFAULT_BUDGET}):");
        Console.WriteLine($"Нажмите Enter, чтобы оставить {InputValidator.DEFAULT_BUDGET}");
    }

    public void ShowPlayerNamePrompt(int playerNumber)
    {
        Console.WriteLine($"Введите имя {(playerNumber == 1 ? "первого" : "второго")} игрока:");
    }

    public void ShowArmyCreationMenu(string playerName)
    {
        Console.Clear();
        Console.WriteLine($"\n{playerName}, как вы хотите создать армию?");
        Console.WriteLine("1 - Вручную");
        Console.WriteLine("2 - Случайно");
    }

    public void ShowArmyCreated(string playerName, bool isManual)
    {
        var armyType = isManual ? "собранная армия" : "случайная армия";
        Console.WriteLine($"\n{playerName}, ваша {armyType}:");
    }

    public void ShowArmyPreviewPause()
    {
        Console.WriteLine("\nНажмите Enter, чтобы продолжить...");
        Console.ReadLine();
    }

    public void ShowTransitionMessage()
    {
        Console.WriteLine("\nНажмите Enter, чтобы перейти к следующему игрока...");
        Console.ReadLine();
    }

    public void ShowGameStartMessage()
    {
        Console.WriteLine("\nАрмии созданы!\n");
    }

    public void ShowBattleStartMessage()
    {
        Console.WriteLine("\nНажмите Enter для начала боя...");
        Console.ReadKey();
    }

    public void ShowGameLoadedMessage()
    {
        Console.WriteLine("\nИгра загружена! Нажмите Enter для продолжения...");
        Console.ReadKey();
    }

    public void ShowGameErrorMessage(string message)
    {
        Console.WriteLine($"\nОшибка: {message}");
        Console.ReadKey();
    }

    public void ShowReturnToMenuMessage()
    {
        Console.WriteLine("\nНажмите Enter для возврата в меню...");
        Console.ReadKey();
    }
}
