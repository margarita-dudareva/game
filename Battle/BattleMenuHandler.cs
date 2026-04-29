using System;

/// <summary>
/// Отвечает за меню и взаимодействие между раундами боя
/// </summary>
public class BattleMenuHandler
{
    private GameManager _gameManager;
    private SaveManager _saveManager;
    private bool _gameSavedDuringBattle;

    public BattleMenuHandler(GameManager gameManager, SaveManager? saveManager = null)
    {
        _gameManager = gameManager ?? throw new ArgumentNullException(nameof(gameManager));
        _saveManager = saveManager ?? gameManager.GetSaveManager();
        _gameSavedDuringBattle = false;
    }

    /// <summary>
    /// Результаты выбора в меню между раундами
    /// </summary>
    public enum RoundMenuChoice
    {
        Continue,           // Продолжить с меню между раундами
        AutoPlayToEnd,      // Проиграть весь остаток без пауз
        ChangeFormation,    // Изменить режим боя
        Exit                // Выйти из игры
    }

    /// <summary>
    /// Был ли произведен save во время боя
    /// </summary>
    public bool WasGameSavedDuringBattle => _gameSavedDuringBattle;

    /// <summary>
    /// Показать меню между раундами и получить выбор
    /// </summary>
    public RoundMenuChoice ShowRoundMenu(Player player1, Player player2, Player currentPlayer, int roundNumber, string gameMode = "BridgeFormation")
    {
        while (true)
        {
            Console.WriteLine("\n╔════════════════════════════════════╗");
            Console.WriteLine("║      МЕНЮ МЕЖДУ РАУНДАМИ           ║");
            Console.WriteLine("╠════════════════════════════════════╣");
            Console.WriteLine("║ 1. Продолжить игру                 ║");
            Console.WriteLine("║ 2. Сохранить игру                  ║");
            Console.WriteLine("║ 3. Сохранить и выйти               ║");
            Console.WriteLine("║ 4. Изменить режим боя              ║");
            Console.WriteLine("║ 5. Проиграть до конца (без пауз)   ║");
            Console.WriteLine("║ 6. Выйти без сохранения            ║");
            Console.WriteLine("╚════════════════════════════════════╝");
            Console.Write("\nВыберите: ");

            string input = Console.ReadLine() ?? "";

            switch (input)
            {
                case "1":
                    return RoundMenuChoice.Continue;
                case "2":
                    SaveGame(player1, player2, currentPlayer, roundNumber, gameMode);
                    return RoundMenuChoice.Continue;
                case "3":
                    SaveGame(player1, player2, currentPlayer, roundNumber, gameMode);
                    return RoundMenuChoice.Exit;
                case "4":
                    return RoundMenuChoice.ChangeFormation;
                case "5":
                    Console.WriteLine("\n⚡ Проигрывание остатка игры без пауз...\n");
                    return RoundMenuChoice.AutoPlayToEnd;
                case "6":
                    Console.WriteLine("\nИгра завершена без сохранения.");
                    return RoundMenuChoice.Exit;
                default:
                    Console.WriteLine("Неверный выбор!");
                    continue;
            }
        }
    }

    /// <summary>
    /// Сохранить текущее состояние игры
    /// </summary>
    private void SaveGame(Player player1, Player player2, Player currentPlayer, int roundNumber, string gameMode = "BridgeFormation")
    {
        Console.Write("\nВведите имя для сохранения (оставьте пусто для автосохранения): ");
        string saveName = Console.ReadLine() ?? "";

        if (string.IsNullOrWhiteSpace(saveName))
        {
            saveName = $"{player1.Name}_vs_{player2.Name}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}";
        }

        try
        {
            var saveData = _gameManager.CreateSaveData(player1, player2, currentPlayer, roundNumber, gameMode);
            _saveManager.SaveGame(saveData, saveName);
            Console.WriteLine($"✓ Игра сохранена: {saveName}");
            _gameSavedDuringBattle = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Ошибка при сохранении: {ex.Message}");
        }
    }

    /// <summary>
    /// Выбрать новый режим боя
    /// </summary>
    public IBattleFormation ChooseFormation()
    {
        var selector = new BattleFormationSelector();
        IBattleFormation newFormation = selector.SelectFormation();
        Console.WriteLine($"\n✓ Режим изменил на: {newFormation.Name}\n");
        return newFormation;
    }
}
