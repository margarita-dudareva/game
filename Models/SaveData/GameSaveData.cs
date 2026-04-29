using System;
using System.Collections.Generic;

/// <summary>
/// Данные для сохранения состояния игры в JSON
/// </summary>
public class GameSaveData
{
    public string Player1Name { get; set; } = "";
    public string Player2Name { get; set; } = "";
    public int RoundNumber { get; set; }
    public string CurrentPlayerName { get; set; } = "";
    public List<UnitSaveData> Player1Units { get; set; } = new();
    public List<UnitSaveData> Player2Units { get; set; } = new();
    
    // Настройки прокси при сохранении
    public bool ProxyDamageLoggingEnabled { get; set; } = false;
    public bool ProxyDeathBeepEnabled { get; set; } = false;
    
    // Номер сессии логирования ущерба (чтобы каждая игра имела свой лог)
    public int DamageLogSessionNumber { get; set; } = 1;
    
    // Режим боя (тип боевого построения)
    public string GameMode { get; set; } = "BridgeFormation";
}

/// <summary>
/// Данные юнита для сохранения
/// </summary>
public class UnitSaveData
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int Attack { get; set; }
    public int Defense { get; set; }
    public int MaxHealth { get; set; }
    public int CurrentHealth { get; set; }
    public int Cost { get; set; }
    public bool IsAlive { get; set; }
    public string UnitType { get; set; } = "";
    // Arrow параметры
    public int? ArrowRange { get; set; }
    public int? ArrowPower { get; set; }
    // Clone (Wizard) параметры
    public int? CloneRange { get; set; }
    public int? CloneProbability { get; set; }
    // Heal (Healer) параметры
    public int? HealRange { get; set; }
    public int? HealPower { get; set; }
}
