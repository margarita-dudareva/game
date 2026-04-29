using System;
using System.Collections.Generic;
using System.Linq;
using Game.Models.Units.Adapters;

/// <summary>
/// Управление состоянием игры (загрузка, сохранение)
/// Делегирует сохранение файлов SaveManager
/// </summary>
public class GameManager
{
    private SaveManager _saveManager;

    /// <summary>
    /// Конструктор с dependency injection SaveManager
    /// </summary>
    public GameManager(SaveManager saveManager)
    {
        _saveManager = saveManager ?? throw new ArgumentNullException(nameof(saveManager));
    }

    public GameManager() : this(new SaveManager())
    {
    }

    /// <summary>
    /// Создать данные для сохранения текущего состояния игры
    /// </summary>
    public GameSaveData CreateSaveData(Player player1, Player player2, Player currentPlayer, int roundNumber, string gameMode = "BridgeFormation")
    {
        var saveData = new GameSaveData
        {
            Player1Name = player1.Name,
            Player2Name = player2.Name,
            RoundNumber = roundNumber,
            CurrentPlayerName = currentPlayer.Name,
            Player1Units = ConvertUnitsToSaveData(player1.Army?.Units ?? new()),
            Player2Units = ConvertUnitsToSaveData(player2.Army?.Units ?? new()),
            ProxyDamageLoggingEnabled = ProxySettings.Current.EnableDamageLogging,
            ProxyDeathBeepEnabled = ProxySettings.Current.EnableDeathBeep,
            DamageLogSessionNumber = DamageLogViewer.GetSessionNumber(),
            GameMode = gameMode
        };

        return saveData;
    }

    /// <summary>
    /// Восстановить игру из сохраненных данных
    /// </summary>
    public (Player player1, Player player2, Player currentPlayer, int roundNumber, string gameMode)? LoadGameState(GameSaveData saveData)
    {
        try
        {
            // Восстанавливаем настройки прокси из сохраненной игры
            ProxySettings.Current.EnableDamageLogging = saveData.ProxyDamageLoggingEnabled;
            ProxySettings.Current.EnableDeathBeep = saveData.ProxyDeathBeepEnabled;
            
            // Восстанавливаем номер сессии логирования, чтобы лог принадлежал сохраненной игре
            DamageLogViewer.SetSessionNumber(saveData.DamageLogSessionNumber);

            var player1 = new Player(saveData.Player1Name);
            var player2 = new Player(saveData.Player2Name);
            
            var army1 = new Army(saveData.Player1Name + " Army");
            var army2 = new Army(saveData.Player2Name + " Army");

            foreach (var unitData in saveData.Player1Units ?? new())
            {
                var unit = CreateUnitFromSaveData(unitData);
                if (unit != null)
                    army1.AddUnit(unit);
            }

            foreach (var unitData in saveData.Player2Units ?? new())
            {
                var unit = CreateUnitFromSaveData(unitData);
                if (unit != null)
                    army2.AddUnit(unit);
            }

            player1.Army = army1;
            player2.Army = army2;

            var currentPlayer = saveData.CurrentPlayerName == player1.Name ? player1 : player2;
            string gameMode = saveData.GameMode ?? "BridgeFormation";

            return (player1, player2, currentPlayer, saveData.RoundNumber, gameMode);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Ошибка при восстановлении игры: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Получить SaveManager для прямого доступа к сохранениям
    /// </summary>
    public SaveManager GetSaveManager() => _saveManager;

    private List<UnitSaveData> ConvertUnitsToSaveData(List<IUnit> units)
    {
        var result = new List<UnitSaveData>();

        foreach (var unit in units)
        {
            var unitData = new UnitSaveData
            {
                Id = unit.Id,
                Name = unit.Name,
                Attack = unit.Attack,
                Defense = unit.Defense,
                MaxHealth = unit.MaxHealth,
                CurrentHealth = unit.CurrentHealth,
                Cost = unit.Cost,
                IsAlive = unit.IsAlive,
                UnitType = GetUnitType(unit)
            };

            if (unit.SpecialAbility is ArrowAbility arrowAbility)
            {
                unitData.ArrowRange = arrowAbility.Range;
                unitData.ArrowPower = arrowAbility.Power;
            }
            else if (unit.SpecialAbility is CloneAbility cloneAbility)
            {
                unitData.CloneRange = cloneAbility.Range;
                unitData.CloneProbability = cloneAbility.CloneProbability;
            }
            else if (unit.SpecialAbility is HealAbility healAbility)
            {
                unitData.HealRange = healAbility.Range;
                unitData.HealPower = healAbility.HealPower;
            }

            result.Add(unitData);
        }

        return result;
    }

    /// <summary>
    /// Создать юнита из сохраненных данных, используя factory pattern
    /// </summary>
    private IUnit? CreateUnitFromSaveData(UnitSaveData unitData)
    {
        IUnit? unit = null;

        if (unitData.UnitType == "LightInfantry")
        {
            unit = new LightInfantry(unitData.Attack, unitData.Defense, unitData.MaxHealth, unitData.Cost);
        }
        else if (unitData.UnitType == "HeavyInfantry")
        {
            unit = new BuffableHeavyInfantry(unitData.Attack, unitData.Defense, unitData.MaxHealth, unitData.Cost);
        }
        else if (unitData.UnitType == "Archer" && unitData.ArrowRange.HasValue && unitData.ArrowPower.HasValue)
        {
            unit = new UnitBase(unitData.Name, unitData.Attack, unitData.Defense, unitData.MaxHealth, unitData.Cost,
                                new ArrowAbility(unitData.ArrowRange.Value, unitData.ArrowPower.Value));
        }
        else if (unitData.UnitType == "Wizard" && unitData.CloneRange.HasValue && unitData.CloneProbability.HasValue)
        {
            unit = new UnitBase(unitData.Name, unitData.Attack, unitData.Defense, unitData.MaxHealth, unitData.Cost,
                                new CloneAbility(unitData.CloneRange.Value, unitData.CloneProbability.Value));
        }
        else if (unitData.UnitType == "Healer" && unitData.HealRange.HasValue && unitData.HealPower.HasValue)
        {
            unit = new UnitBase(unitData.Name, unitData.Attack, unitData.Defense, unitData.MaxHealth, unitData.Cost,
                                new HealAbility(unitData.HealRange.Value, unitData.HealPower.Value));
        }
        else if (unitData.UnitType == "Gulyay-Gorod")
        {
            unit = new GulyayGorodAdapter(unitData.Attack, unitData.Defense, unitData.MaxHealth, unitData.Cost);
        }

        if (unit != null)
        {
            // Восстанавливаем сохраненный ID
            unit.SetId(unitData.Id);
            // Восстанавливаем HP (нужно нанести урон, чтобы получить нужный CurrentHealth)
            unit.TakeDamage(unit.MaxHealth - unitData.CurrentHealth);
            
            // Подписываем наблюдателей согласно настройкам
            if (ProxySettings.Current.EnableDamageLogging)
            {
                unit.Subscribe(new DamageLoggerObserver());
            }

            if (ProxySettings.Current.EnableDeathBeep)
            {
                unit.Subscribe(new DeathBeepObserver());
            }
        }

        return unit;
    }

    private string GetUnitType(IUnit unit)
    {
        // Проверяем по имени юнита, так как это работает даже с Proxy-объектами
        if (unit.Name == "Light Infantry")
            return "LightInfantry";
        else if (unit.Name == "Heavy Infantry")
            return "HeavyInfantry";
        else if (unit.SpecialAbility is ArrowAbility)
            return "Archer";
        else if (unit.SpecialAbility is CloneAbility)
            return "Wizard";
        else if (unit.SpecialAbility is HealAbility)
            return "Healer";
        else if (unit.Name == "Gulyay-Gorod")
            return "Gulyay-Gorod";
        return "Unknown";
    }
}
