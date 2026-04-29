using System;
using System.Collections.Generic;

/// <summary>
/// Интерфейс для отображения информации о бое
/// Отделяет логику боя от UI
/// </summary>
public interface IBattleDisplay
{
    /// <summary>
    /// Показать начало боя
    /// </summary>
    void ShowBattleStart(string firstPlayerName);

    /// <summary>
    /// Показать раунд
    /// </summary>
    void ShowRound(int roundNumber, int player1UnitsCount, int player2UnitsCount, string player1Name, string player2Name);

    /// <summary>
    /// Показать состояние армий
    /// </summary>
    void DisplayArmiesStatus(string currentPlayerName, string armyVisualization1, string allUnitsHealth1, string allUnitsAbilities1,
                              string opponentName, string armyVisualization2, string allUnitsHealth2, string allUnitsAbilities2,
                              List<string>? allUnitsBuffs1 = null, List<string>? allUnitsBuffs2 = null);

    /// <summary>
    /// Показать статистику боя
    /// </summary>
    void DisplayGameStatistics(string winnerName, int roundNumber, List<(string Name, int CurrentHealth, int MaxHealth)> aliveUnits);

    /// <summary>
    /// Показать статистику ничьи (10+ раундов без изменений)
    /// </summary>
    void DisplayDrawStatistics(int roundNumber);

    /// <summary>
    /// Показать информацию о выстреле лучника
    /// </summary>
    void ShowArcherAttackInfo(string archerName, int archerPosition, int range, List<string> targetOptions);

    /// <summary>
    /// Показать результат выстрела
    /// </summary>
    void ShowArcherAttackResult(string archerName, int archerId, string targetName, int targetId, int damageDealt, int currentHealth, int maxHealth);

    /// <summary>
    /// Показать, что лучник первый в армии и не может стрелять
    /// </summary>
    void ShowArcherCannotShootFirst(string archerName, int archerPosition);

    /// <summary>
    /// Показать, что у лучника маленький радиус и нет целей
    /// </summary>
    void ShowArcherNoTargets(string archerName, int archerPosition, int range);

    /// <summary>
    /// Показать информацию о лечении лекаря
    /// </summary>
    void ShowHealerAttackInfo(string healerName, int healerPosition, int range, List<string> targetOptions);

    /// <summary>
    /// Показать результат лечения
    /// </summary>
    void ShowHealerAttackResult(string targetName, int targetPosition, int healAmount, int currentHealth, int maxHealth);

    /// <summary>
    /// Показать информацию о клонировании волшебника
    /// </summary>
    /// <summary>
    /// Показать попытку клонирования
    /// </summary>
    void ShowWizardCloneAttempt(string wizardName, int wizardPosition, string targetName, int cloneProbability);

    /// <summary>
    /// Показать успешное клонирование
    /// </summary>
    void ShowWizardCloneSuccess(string originalUnitName, int originalPosition, string cloneName);

    /// <summary>
    /// Показать неудачное клонирование
    /// </summary>
    void ShowWizardCloneFailed(string targetUnitName, int targetPosition, int cloneProbability);

    /// <summary>
    /// Показать, что у волшебника нет целей для клонирования
    /// </summary>
    void ShowWizardNoTargets(string wizardName, int wizardPosition);

    /// <summary>
    /// Показать, что нет никого для лечения
    /// </summary>
    void ShowHealerNoTargets(string healerName, int healerPosition);

    /// <summary>
    /// Показать, что юнит умер
    /// </summary>
    void ShowUnitDefeated(string unitName, string reason);

    /// <summary>
    /// Показать информацию о выходе нового юнита
    /// </summary>
    void ShowNewUnitAppears(string unitName);

    /// <summary>
    /// Показать, что оруженосец не нашел цели
    /// </summary>
    void ShowSquireNoTargets(string squireName, int squireId);

    /// <summary>
    /// Показать попытку оруженосца применить бафф
    /// </summary>
    void ShowSquireBuffAttempt(string squireName, int squireId, string targetName, int targetId, int probability);

    /// <summary>
    /// Показать успешное наложение баффа оруженосцем
    /// </summary>
    void ShowSquireBuffSuccess(string squireName, int squireId, string targetName, int targetId, string buffName);

    /// <summary>
    /// Показать неудачную попытку оруженосца
    /// </summary>
    void ShowSquireBuffFailed(string squireName, int squireId, int probability);

    /// <summary>
    /// Показать, что бафф слетел при ударе
    /// </summary>
    void ShowBuffLost(string unitName);
}
