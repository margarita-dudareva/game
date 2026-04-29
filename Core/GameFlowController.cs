using System;

/// <summary>
/// Управление потоком игры (новая игра, загрузка, выход)
/// </summary>
public class GameFlowController
{
    private MenuManager _menuManager;
    private InputValidator _inputValidator;
    private GameManager _gameManager;
    private SaveManager _saveManager;
    private ArmyCreationManager _armyCreationManager;
    private ArmyDisplayManager _armyDisplayManager;
    private string? _loadedSaveName;

    public GameFlowController()
    {
        _saveManager = new SaveManager();
        _menuManager = new MenuManager();
        _inputValidator = new InputValidator();
        _gameManager = new GameManager(_saveManager);
        _armyCreationManager = new ArmyCreationManager(_inputValidator);
        _armyDisplayManager = new ArmyDisplayManager();
    }

    /// <summary>
    /// Показать главное меню и получить выбор
    /// </summary>
    public int ShowMainMenu()
    {
        _menuManager.ShowMainMenu();
        int choice = _menuManager.GetMenuChoice();
        
        if (choice < 1 || choice > 4)
        {
            _menuManager.ShowInvalidChoiceMessage();
            return -1;
        }

        return choice;
    }

    /// <summary>
    /// Настройки проксирования юнитов
    /// </summary>
    public void ConfigureProxySettings()
    {
        bool damageLogging = _inputValidator.GetYesNo("Включить логирование урона (имена юнитов и дмг) ? (Y/N): ");
        bool deathBeep = _inputValidator.GetYesNo("Включить звук при смерти юнита (Beep) ? (Y/N): ");

        ProxySettings.Current.EnableDamageLogging = damageLogging;
        ProxySettings.Current.EnableDeathBeep = deathBeep;
        ProxySettings.Current.SaveToFile();

        Console.WriteLine($"Настройки прокси: логирование = {(damageLogging ? "Вкл" : "Выкл")}, сигнал при смерти = {(deathBeep ? "Вкл" : "Выкл")}.");
        Console.WriteLine("Настройки применяются к новым создаваемым юнитам.");
        Console.WriteLine("Нажмите Enter для возврата в меню...");
        Console.ReadLine();
    }

    /// <summary>
    /// Начать новую игру
    /// </summary>
    public void StartNewGame()
    {
        DamageLogViewer.ClearLog();

        string player1Name = _inputValidator.GetPlayerName(1);
        string player2Name = _inputValidator.GetPlayerName(2);
        var player1 = new Player(player1Name);
        var player2 = new Player(player2Name);

        int budget = _inputValidator.GetBudget();

        _menuManager.ShowArmyCreationMenu(player1Name);
        var mode1 = _inputValidator.GetArmyCreationMode();
        bool isManual1 = mode1 == 1;
        player1.Army = _armyCreationManager.CreateArmy(player1Name, budget, isManual1);
        _menuManager.ShowArmyCreated(player1Name, isManual1);
        _armyDisplayManager.ShowArmyInfo(player1);
        if (!isManual1)
        {
            _menuManager.ShowArmyPreviewPause();
        }

        _menuManager.ShowTransitionMessage();
        Console.Clear();

        _menuManager.ShowArmyCreationMenu(player2Name);
        var mode2 = _inputValidator.GetArmyCreationMode();
        bool isManual2 = mode2 == 1;
        player2.Army = _armyCreationManager.CreateArmy(player2Name, budget, isManual2);
        _menuManager.ShowArmyCreated(player2Name, isManual2);
        _armyDisplayManager.ShowArmyInfo(player2);
        if (!isManual2)
        {
            _menuManager.ShowArmyPreviewPause();
        }

        _menuManager.ShowGameStartMessage();
        _armyDisplayManager.DisplayArmies(player1, player2);

        _menuManager.ShowBattleStartMessage();

        var battle = new Battle(player1, player2, _gameManager);
        var winner = battle.Start();

        if (winner != null)
        {
            if (!ShowGameEndMenu())
            {
                _menuManager.ShowReturnToMenuMessage();
            }
        }
    }

    /// <summary>
    /// Показать меню в конце игры (просмотр логирования если включено)
    /// Возвращает true если были показаны логи (и уже показано "Нажмите Enter")
    /// </summary>
    private bool ShowGameEndMenu()
    {
        if (!ProxySettings.Current.EnableDamageLogging)
            return false;

        Console.WriteLine();
        if (_inputValidator.GetYesNo("Хотите посмотреть лог урона? (Y/N): "))
        {
            DamageLogViewer.DisplayLog();
            Console.WriteLine("\nНажмите Enter для возврата в меню...");
            Console.ReadKey();
            return true; // Логи были показаны
        }
        
        return false; // Логи не просматривались
    }

    /// <summary>
    /// Загрузить сохраненную игру
    /// </summary>
    public void LoadSavedGame()
    {
        var saveData = LoadGameWithSelection();

        if (saveData == null)
        {
            Console.ReadKey();
            return;
        }

        try
        {
            var gameState = _gameManager.LoadGameState(saveData);
            if (gameState == null)
            {
                _menuManager.ShowGameErrorMessage("при загрузке игры");
                return;
            }

            var (player1, player2, currentPlayer, roundNumber, gameMode) = gameState.Value;

            _menuManager.ShowGameLoadedMessage();

            var battle = new Battle(player1, player2, _gameManager);
            battle.SetupLoadedGame(currentPlayer, roundNumber, gameMode);
            var winner = battle.Start();

            if (winner != null)
            {
                if (!ShowGameEndMenu())
                {
                    _menuManager.ShowReturnToMenuMessage();
                }
                
                // Удаляем сохранение если игра была проиграна до конца без дополнительных сохранений
                if (!string.IsNullOrEmpty(_loadedSaveName) && !battle.MenuHandler.WasGameSavedDuringBattle)
                {
                    DeleteLoadedSave();
                }
            }
        }
        catch (Exception ex)
        {
            _menuManager.ShowGameErrorMessage($"при восстановлении игры: {ex.Message}");
        }
    }

    /// <summary>
    /// Показать меню для загрузки сохранённой игры
    /// </summary>
    private GameSaveData? LoadGameWithSelection()
    {
        var saves = _saveManager.GetSavesList();

        if (saves.Count == 0)
        {
            Console.WriteLine("Нет сохраненных игр.");
            return null;
        }

        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════╗");
        Console.WriteLine("║   ЗАГРУЗИТЬ СОХРАНЕННУЮ ИГРУ       ║");
        Console.WriteLine("╚════════════════════════════════════╝\n");

        for (int i = 0; i < saves.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {saves[i]}");
        }

        Console.WriteLine($"\n{saves.Count + 1}. Вернуться в меню");
        Console.Write("\nВыберите сохранение: ");

        if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= saves.Count)
        {
            try
            {
                string selectedSave = saves[choice - 1];
                var saveData = _saveManager.LoadGame(selectedSave);
                if (saveData != null)
                {
                    _loadedSaveName = selectedSave;
                    Console.WriteLine($"Игра загружена: {selectedSave}");
                }
                return saveData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке: {ex.Message}");
                return null;
            }
        }

        return null;
    }

    /// <summary>
    /// Удалить загруженное сохранение при завершении игры
    /// </summary>
    private void DeleteLoadedSave()
    {
        try
        {
            _saveManager.DeleteGame(_loadedSaveName!);
            Console.WriteLine($"\n✓ Сохранение \"{_loadedSaveName}\" удалено (игра была проиграна до конца)");
            _loadedSaveName = null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠ Не удалось удалить сохранение: {ex.Message}");
        }
    }
}
