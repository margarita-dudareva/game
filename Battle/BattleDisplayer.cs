using System;
using System.Collections.Generic;
using System.Linq;




/// <summary>
/// Отвечает за отображение информации о боевой системе
/// Отделена от логики боя
/// </summary>
public class BattleDisplayer : IBattleDisplay
{
    public void ShowBattleStart(string firstPlayerName)
    {
        try { Console.Clear(); } catch { }
        Console.WriteLine(new string('=', 63));
        Console.WriteLine("===                   НАЧАЛО БИТВЫ                          ===");
        Console.WriteLine(new string('=', 63) + "\n");
        Console.WriteLine($"Первым ходит: {firstPlayerName}\n");
        Console.ReadKey();
    }

    public void ShowRound(int roundNumber, int player1UnitsCount, int player2UnitsCount, string player1Name, string player2Name)
    {
        try { Console.Clear(); } catch { }
        Console.WriteLine(new string('=', 63));
        Console.WriteLine($"===                  РАУНД {roundNumber}                   ===");
        Console.WriteLine($"{player1Name}: {player1UnitsCount} юнитов");
        Console.WriteLine($"{player2Name}: {player2UnitsCount} юнитов");
        Console.WriteLine(new string('=', 63));
    }

    public void DisplayArmiesStatus(string currentPlayerName, string armyVisualization1, string allUnitsHealth1, string allUnitsAbilities1,
                                     string opponentName, string armyVisualization2, string allUnitsHealth2, string allUnitsAbilities2,
                                     List<string>? allUnitsBuffs1 = null, List<string>? allUnitsBuffs2 = null)
    {
        Console.WriteLine("\n" + new string('=', 63));
        Console.WriteLine($"{currentPlayerName}: {armyVisualization1}");
        Console.WriteLine($"{currentPlayerName}:{allUnitsHealth1}");
        Console.WriteLine($"{currentPlayerName}:{allUnitsAbilities1}");
        
        // Вывести баффы если они есть
        if (allUnitsBuffs1 != null && allUnitsBuffs1.Count > 0)
        {
            foreach (var buffLine in allUnitsBuffs1)
            {
                Console.WriteLine($"{currentPlayerName}:{buffLine}");
            }
        }
        
        Console.WriteLine(new string('=', 63));
        Console.WriteLine($"{opponentName}: {armyVisualization2}");
        Console.WriteLine($"{opponentName}:{allUnitsHealth2}");
        Console.WriteLine($"{opponentName}:{allUnitsAbilities2}");
        
        // Вывести баффы если они есть
        if (allUnitsBuffs2 != null && allUnitsBuffs2.Count > 0)
        {
            foreach (var buffLine in allUnitsBuffs2)
            {
                Console.WriteLine($"{opponentName}:{buffLine}");
            }
        }
        
        Console.WriteLine(new string('=', 63));
    }

    public void DisplayGameStatistics(string winnerName, int roundNumber, List<(string Name, int CurrentHealth, int MaxHealth)> aliveUnits)
    {
        Console.WriteLine("\n" + new string('=', 63));
        Console.WriteLine($"===                СТАТИСТИКА БОЯ                            ===");
        Console.WriteLine(new string('=', 63));
        Console.WriteLine($"Всего раундов: {roundNumber,-27} =");
        Console.WriteLine("\n");
        Console.WriteLine($"ПОБЕДИТЕЛЬ: {winnerName,-27} =");
        Console.WriteLine(new string('=', 66));
        Console.WriteLine($"              Оставшаяся армия:                      ");
        
        if (aliveUnits.Count > 0)
        {
            foreach (var (name, currentHealth, maxHealth) in aliveUnits)
            {
                string unitInfo = $"  {name} HP:{currentHealth}/{maxHealth}";
                Console.WriteLine($"║ {unitInfo,-38} ║");
            }
        }
        else
        {
            Console.WriteLine("Нет живых юнитов                       ");
        }
        
        Console.WriteLine(new string('=', 63));
    }

    /// <summary>
    /// Вывести статистику при ничье (10+ раундов без изменений)
    /// </summary>
    public void DisplayDrawStatistics(int roundNumber)
    {
        Console.WriteLine("\n" + new string('=', 63));
        Console.WriteLine($"===                СТАТИСТИКА БОЯ                            ===");
        Console.WriteLine(new string('=', 63));
        Console.WriteLine($"Всего раундов: {roundNumber,-27} =");
        Console.WriteLine("\n");
        Console.WriteLine($"РЕЗУЛЬТАТ: НИЧЬЯ                                  =");
        Console.WriteLine($"Причина: 10+ раундов без изменений                 =");
        Console.WriteLine(new string('=', 63));
    }

    public void ShowArcherAttackInfo(string archerName, int archerPosition, int range, List<string> targetOptions)
    {
        Console.WriteLine($"\n№{archerPosition} {archerName} может стрелять!");
        Console.WriteLine($"Дальность: {range} рядов впереди себя");
        Console.WriteLine("\nДоступные цели:");
        
        for (int i = 0; i < targetOptions.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {targetOptions[i]}");
        }
    }

    public void ShowArcherAttackResult(string archerName, int archerId, string targetName, int targetId, int damageDealt, int currentHealth, int maxHealth)
    {
        Console.WriteLine($"\nЛучник №{archerId} ({archerName}) стреляет в №{targetId} ({targetName})!");
        Console.WriteLine($"Урон: {damageDealt}");
        Console.WriteLine($"Здоровье цели: {currentHealth}/{maxHealth}");
    }

    public void ShowArcherCannotShootFirst(string archerName, int archerPosition)
    {
        Console.WriteLine($"\n№{archerPosition} {archerName} первый в армии, не может стрелять.");
    }

    public void ShowArcherNoTargets(string archerName, int archerPosition, int range)
    {
        Console.WriteLine($"\n№{archerPosition} {archerName} имеет маленький радиус ({range}), нет целей для стрельбы.");
    }

    public void ShowHealerAttackInfo(string healerName, int healerPosition, int range, List<string> targetOptions)
    {
        Console.WriteLine($"\n№{healerPosition} {healerName} может лечить!");
        Console.WriteLine($"Радиус действия: {range} рядов вокруг себя");
        Console.WriteLine("\nДоступные цели:");
        
        for (int i = 0; i < targetOptions.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {targetOptions[i]}");
        }
    }

    public void ShowHealerAttackResult(string targetName, int targetPosition, int healAmount, int currentHealth, int maxHealth)
    {
        Console.WriteLine($"\nЮнит №{targetPosition} ({targetName}) вылечен на {healAmount} HP.");
        Console.WriteLine($"Здоровье: {currentHealth}/{maxHealth}");
    }

    public void ShowWizardCloneAttempt(string wizardName, int wizardPosition, string targetName, int cloneProbability)
    {
        Console.WriteLine($"\n№{wizardPosition} {wizardName} пытается клонировать {targetName} (вероятность {cloneProbability}%)...");
    }

    public void ShowWizardCloneSuccess(string originalUnitName, int originalPosition, string cloneName)
    {
        Console.WriteLine($"\n✓ Клонирование успешно!");
        Console.WriteLine($"Волшебник создал копию юнита №{originalPosition} ({originalUnitName})");
        Console.WriteLine($"Новый клон добавлен в армию!");
    }

    public void ShowWizardCloneFailed(string targetUnitName, int targetPosition, int cloneProbability)
    {
        Console.WriteLine($"\n✗ Клонирование не удалось!");
        Console.WriteLine($"Волшебник попытался клонировать юнита №{targetPosition} ({targetUnitName}), но магия не сработала...");
    }

    public void ShowWizardNoTargets(string wizardName, int wizardPosition)
    {
        Console.WriteLine($"\n№{wizardPosition} {wizardName} не нашел никого для клонирования.");
    }

    public void ShowHealerNoTargets(string healerName, int healerPosition)
    {
        Console.WriteLine($"\n№{healerPosition} {healerName} не нашел никого, кого нужно лечить.");
    }

    public void ShowUnitDefeated(string unitName, string reason)
    {
        Console.WriteLine($"{unitName} {reason}!");
    }

    public void ShowNewUnitAppears(string unitName)
    {
        Console.WriteLine($"На поле выходит {unitName}!");
    }

    /// <summary>
    /// Получить визуализацию армии с ID номерами в скобках (в обратном порядке)
    /// A - Archer, H - Heavy, L - Light, E - Healer, W - Wizard
    /// Формат: L(37)   A(26)   V(2)   и т.д.
    /// </summary>
    public string GetArmyVisualization(IArmy army)
    {
        var symbols = new List<string>();
        
        // Показываем юнитов в обратном порядке (для удобного чтения слева направо)
        for (int i = army.Units.Count - 1; i >= 0; i--)
        {
            var unit = army.Units[i];
            if (unit.IsAlive)
            {
                string unitSymbol = $"{unit.GetDisplaySymbol()}({unit.Id})".PadRight(8);
                symbols.Add(unitSymbol);
            }
        }
        return string.Concat(symbols);
    }

    /// <summary>
    /// Получить информацию о здоровье текущего юнита
    /// </summary>
    public string GetCurrentUnitHealth(IArmy army)
    {
        var currentUnit = army.GetCurrentUnit();
        if (currentUnit != null && currentUnit.IsAlive)
        {
            return $"{currentUnit.Name}: {currentUnit.CurrentHealth}/{currentUnit.MaxHealth}";
        }
        return "Нет живых юнитов";
    }

    /// <summary>
    /// Получить информацию о здоровье всех юнитов армии в виде строки
    /// Каждое значение HP выровнено в колонку шириной 8 символов
    /// </summary>
    public string GetAllUnitsHealth(IArmy army)
    {
        var healthStrings = new List<string>();
        for (int i = army.Units.Count - 1; i >= 0; i--)
        {
            var unit = army.Units[i];
            if (unit.IsAlive)
            {
                healthStrings.Add($"{unit.CurrentHealth}/{unit.MaxHealth}".PadRight(8));
            }
        }
        return healthStrings.Count > 0 ? string.Concat(healthStrings) : "Нет живых юнитов";
    }

    /// <summary>
    /// Получить информацию о способностях всех юнитов армии в виде строки
    /// </summary>
    public string GetAllUnitsAbilities(IArmy army)
    {
        var abilityStrings = new List<string>();
        for (int i = army.Units.Count - 1; i >= 0; i--)
        {
            var unit = army.Units[i];
            if (unit.IsAlive)
            {
                string abilityInfo = "-";
                if (unit.SpecialAbility is ArrowAbility arrowAbility)
                {
                    abilityInfo = $"Стр:{arrowAbility.Power}";
                }
                else if (unit.SpecialAbility is HealAbility healAbility)
                {
                    abilityInfo = $"Леч:{healAbility.HealPower}";
                }
                else if (unit.SpecialAbility is CloneAbility cloneAbility)
                {
                    abilityInfo = $"Клон:{cloneAbility.CloneProbability}%";
                }
                else if (unit.SpecialAbility is SquireAbility squireAbility)
                {
                    abilityInfo = $"Буфф:{squireAbility.ActivationProbability}%";
                }
                abilityStrings.Add(abilityInfo.PadRight(8));
            }
        }
        return abilityStrings.Count > 0 ? string.Concat(abilityStrings) : "Нет живых юнитов";
    }

    /// <summary>
    /// Получить информацию о баффах всех юнитов армии в виде многострочной строки
    /// Каждый юнит может иметь несколько баффов, выводятся в столбик (каждый с новой строки)
    /// </summary>
    public List<string> GetAllUnitsBuffs(IArmy army)
    {
        var buffLines = new List<string>();

        // Собрать баффы для каждого юнита
        var unitBuffs = new List<List<string>>();
        for (int i = army.Units.Count - 1; i >= 0; i--)
        {
            var unit = army.Units[i];
            var buffs = new List<string>();

            if (unit.IsAlive && unit is IBuffable buffableUnit)
            {
                var activeBuffs = buffableUnit.GetActiveBuffs();
                foreach (var buff in activeBuffs)
                {
                    string buffShort = buff.Name switch
                    {
                        "Shield" => $"Shi(+{buff.DefenseModifier})",
                        "Spear" => $"Spe(+{buff.AttackModifier})",
                        "Horse" => $"Hor(+{buff.AttackModifier})",
                        "Helmet" => $"Hel(+{buff.ArrowDefenseModifier})",
                        _ => "???"
                    };
                    buffs.Add(buffShort);
                }
            }

            unitBuffs.Add(buffs);
        }

        // Найти максимальное количество баффов у одного юнита
        int maxBuffCount = unitBuffs.Max(b => b.Count);

        // Если нет баффов, вернуть пустой список
        if (maxBuffCount == 0)
        {
            return buffLines;
        }

        // Построить многострочное представление
        for (int buffIndex = 0; buffIndex < maxBuffCount; buffIndex++)
        {
            var buffLine = new List<string>();
            for (int unitIndex = 0; unitIndex < unitBuffs.Count; unitIndex++)
            {
                var unitBuffList = unitBuffs[unitIndex];
                if (buffIndex < unitBuffList.Count)
                {
                    buffLine.Add(unitBuffList[buffIndex].PadRight(12));
                }
                else
                {
                    buffLine.Add("-".PadRight(12));
                }
            }
            buffLines.Add(string.Concat(buffLine));
        }

        return buffLines;
    }

    /// <summary>
    /// Показать заголовок обновленного статуса после хода
    /// </summary>
    public void ShowStatusUpdate()
    {
        Console.WriteLine("\n" + new string('=', 63));
        Console.WriteLine("===               СОСТОЯНИЕ ПОСЛЕ ХОДА                     ===");
        Console.WriteLine(new string('=', 63));
    }

    /// <summary>
    /// Оруженосец не нашел цели
    /// </summary>
    public void ShowSquireNoTargets(string squireName, int squireId)
    {
        Console.WriteLine($"\n№{squireId} {squireName} (оруженосец) нет соседних тяжелых воинов для буффа.");
    }

    /// <summary>
    /// Попытка оруженосца применить бафф
    /// </summary>
    public void ShowSquireBuffAttempt(string squireName, int squireId, string targetName, int targetId, int probability)
    {
        Console.WriteLine($"\n№{squireId} {squireName} (оруженосец) пытается наложить бафф на №{targetId} {targetName}!");
        Console.WriteLine($"Вероятность: {probability}%");
    }

    /// <summary>
    /// Бафф успешно наложен
    /// </summary>
    public void ShowSquireBuffSuccess(string squireName, int squireId, string targetName, int targetId, string buffName)
    {
        Console.WriteLine($"✓ Успех! №{targetId} {targetName} получил бафф: {buffName}!");
    }

    /// <summary>
    /// Бафф не сработал
    /// </summary>
    public void ShowSquireBuffFailed(string squireName, int squireId, int probability)
    {
        Console.WriteLine($"✗ Попытка не удалась (вероятность была {probability}%)");
    }

    /// <summary>
    /// Бафф слетел
    /// </summary>
    public void ShowBuffLost(string unitName)
    {
        Console.WriteLine($"⚠ {unitName} потеряет один бафф от удара противника!");
    }


    private string CenterText(string text, int width)
    {
        if (text.Length > width)
            text = text.Substring(0, width); // обрезка

        int totalPadding = width - text.Length;
        int leftPadding = totalPadding / 2;

        return new string(' ', leftPadding) + text + new string(' ', totalPadding - leftPadding);
    }

    public string DisplayBridgeFormation(IArmy army)
{
    var aliveUnits = army.Units.Where(u => u.IsAlive).ToList();
    const int COLUMN_WIDTH = 12;

    string line1 = ""; // символы
    string line2 = ""; // здоровье
    string line3 = ""; // короткие способности
    string extraInfo = ""; // длинные способности (перенос)

    for (int i = 0; i < aliveUnits.Count; i++)
    {
        var unit = aliveUnits[i];

        string symbol = unit.GetDisplaySymbol().ToString();
        string health = $"{unit.CurrentHealth}/{unit.MaxHealth}";
        string ability = GetUnitInfoWithAbilitiesAndBuffs(unit)
                            .Replace("(", "")
                            .Replace(")", "");

        // если длинная строка → вынести вниз
        if (ability.Length > COLUMN_WIDTH)
        {
            extraInfo += CenterText(ability, COLUMN_WIDTH);
            ability = ""; // в основной строке пусто
        }
        else if (string.IsNullOrWhiteSpace(ability))
        {
            ability = "-";
        }

        line1 += CenterText(symbol, COLUMN_WIDTH);
        line2 += CenterText(health, COLUMN_WIDTH);
        line3 += CenterText(ability, COLUMN_WIDTH);
    }

    string result = $"{line1}\n{line2}\n{line3}";

    if (!string.IsNullOrWhiteSpace(extraInfo))
    {
        result += "\n" + extraInfo;
    }

    return result;
}

    /// <summary>
    /// Вывести армию в режиме 3 на 3
    /// </summary>
    public string DisplayThreeOnThreeFormation(IArmy army)
    {
        const int UNITS_PER_ROW = 3;
        var aliveUnits = army.Units.Where(u => u.IsAlive).ToList();

        string result = "\n╔═══════════════════════════════════════════════════════════════╗\n║  РЯД 1 (НА РИНГЕ):\n";
        
        // Ряд 1 (на ринге)
        for (int i = 0; i < UNITS_PER_ROW && i < aliveUnits.Count; i++)
        {
            var unit = aliveUnits[i];
            string health = $"{unit.CurrentHealth}/{unit.MaxHealth}";
            string abilityBuffInfo = GetUnitInfoWithAbilitiesAndBuffs(unit);
            result += $"║  [{i}] {unit.Name} {health}{abilityBuffInfo}\n";
            
            // Добавить баффы на отдельной строке если они есть
            string buffLine = GetBuffInfoLine(unit);
            if (!string.IsNullOrEmpty(buffLine))
            {
                result += $"║       {buffLine}\n";
            }
        }

        result += "╠═══════════════════════════════════════════════════════════════╣\n";

        // Режим (в запасе)
        int rowNum = 2;
        for (int i = UNITS_PER_ROW; i < aliveUnits.Count; i++)
        {
            if (i % UNITS_PER_ROW == 0)
            {
                if (i > UNITS_PER_ROW) result += "║\n";
                result += $"║  РЯД {rowNum} (ЗАПАС):\n";
                rowNum++;
            }
            var unit = aliveUnits[i];
            string health = $"{unit.CurrentHealth}/{unit.MaxHealth}";
            string abilityBuffInfo = GetUnitInfoWithAbilitiesAndBuffs(unit);
            result += $"║  [{i}] {unit.Name} {health}{abilityBuffInfo}\n";
            
            // Добавить баффы на отдельной строке если они есть
            string buffLine = GetBuffInfoLine(unit);
            if (!string.IsNullOrEmpty(buffLine))
            {
                result += $"║       {buffLine}\n";
            }
        }

        result += "╚═══════════════════════════════════════════════════════════════╝";
        return result;
    }

    /// <summary>
    /// Получить информацию о юните с абилити и баффами в скобках
    /// </summary>
    private string GetUnitInfoWithAbilitiesAndBuffs(IUnit unit)
    {
        // Добавить информацию об абилити ТОЛЬКО (без баффов)
        if (unit.SpecialAbility != null)
        {
            string abilityInfo = unit.SpecialAbility switch
            {
                HealAbility ha => $" (Hel:{ha.HealPower})",
                ArrowAbility => $" (Стр:{unit.Attack})",
                CloneAbility ca => $" (Клон:{ca.CloneProbability}%)",
                SquireAbility sa => $" (Буфф:{sa.ActivationProbability}%)",
                _ => " (Способность)"
            };
            return abilityInfo;
        }

        return "";
    }

    /// <summary>
    /// Получить информацию о баффах юнита для отдельной строки (если есть)
    /// </summary>
    private string GetBuffInfoLine(IUnit unit)
    {
        if (unit is IBuffable buffable)
        {
            var buffs = buffable.GetActiveBuffs();
            if (buffs.Count > 0)
            {
                var buffList = new List<string>();
                foreach (var buff in buffs)
                {
                    string buffShort = buff.Name switch
                    {
                        "Shield" => $"щит(+{buff.DefenseModifier})",
                        "Spear" => $"коп(+{buff.AttackModifier})",
                        "Horse" => $"конь(+{buff.AttackModifier}/{buff.DefenseModifier})",
                        "Helmet" => $"шлем(+{buff.ArrowDefenseModifier})",
                        _ => buff.Name
                    };
                    buffList.Add(buffShort);
                }
                return string.Join(" ", buffList);
            }
        }
        return "";
    }

    /// <summary>
    /// Получить полную информацию о юните для 3 на 3: здоровье, абилити и баффы в одной строке
    /// </summary>
    private string GetUnitInfoFullInline(IUnit unit)
    {
        var extras = new List<string>();

        // Добавить информацию об абилити
        if (unit.SpecialAbility != null)
        {
            string abilityInfo = unit.SpecialAbility switch
            {
                HealAbility ha => $"Hel:{ha.HealPower}",
                ArrowAbility => $"Стр:{unit.Attack}",
                CloneAbility ca => $"Клон:{ca.CloneProbability}%",
                SquireAbility sa => $"Буфф:{sa.ActivationProbability}%",
                _ => "Способность"
            };
            extras.Add(abilityInfo);
        }

        // Добавить информацию о баффах
        if (unit is IBuffable buffable)
        {
            var buffs = buffable.GetActiveBuffs();
            foreach (var buff in buffs)
            {
                string buffShort = buff.Name switch
                {
                    "Shield" => $"щит(+{buff.DefenseModifier})",
                    "Spear" => $"коп(+{buff.AttackModifier})",
                    "Horse" => $"конь(+{buff.AttackModifier}/{buff.DefenseModifier})",
                    "Helmet" => $"шлем(+{buff.ArrowDefenseModifier})",
                    _ => buff.Name
                };
                extras.Add(buffShort);
            }
        }

        // Возвращаем все в скобках на одной строке
        if (extras.Count > 0)
        {
            return $" ({string.Join(" ", extras)})";
        }

        return "";
    }

    /// <summary>
    /// Вывести армию в режиме стенка на стенку
    /// </summary>
    public string DisplayWallToWallFormation(IArmy army)
    {
        var aliveUnits = army.Units.Where(u => u.IsAlive).ToList();

        string result = "\n═══════════════════════════════════════════════════════════════\n";
        result += "  ФРОНТАЛЬНАЯ СТЕНКА:\n";
        result += "═══════════════════════════════════════════════════════════════\n";

        for (int i = 0; i < aliveUnits.Count; i++)
        {
            var unit = aliveUnits[i];
            string abilityBuffInfo = GetUnitInfoWithAbilitiesAndBuffs(unit);
            string healthStr = $"{unit.CurrentHealth}/{unit.MaxHealth}";
            result += $"║  позиция #{i}: {unit.Name} {healthStr}{abilityBuffInfo}\n";
            
            // Добавить баффы на отдельной строке если они есть
            string buffLine = GetBuffInfoLine(unit);
            if (!string.IsNullOrEmpty(buffLine))
            {
                result += $"║       {buffLine}\n";
            }
        }

        result += "═══════════════════════════════════════════════════════════════";
        return result;
    }

    /// <summary>
    /// Вывести боевые позиции для режима 3 на 3 (противниковые пары)
    /// </summary>
    public string DisplayThreeOnThreeMatchups(IBattleFormation formation, IArmy army1, IArmy army2, string player1Name, string player2Name)
    {
        const int UNIT_WIDTH = 15;
        string result = "\n🥊 БОЕВЫЕ ПАРЫ (3 на 3):\n";
        result += $"┌{new string('─', UNIT_WIDTH)}┬{new string('─', UNIT_WIDTH)}┐\n";

        var ringUnits1 = formation.GetRingUnits(army1);
        var ringUnits2 = formation.GetRingUnits(army2);

        for (int i = 0; i < Math.Max(ringUnits1.Count, ringUnits2.Count); i++)
        {
            string unit1 = i < ringUnits1.Count ? ringUnits1[i].Name : "---";
            string unit2 = i < ringUnits2.Count ? ringUnits2[i].Name : "---";

            result += $"│ {unit1,-13} │ {unit2,-13} │\n";
        }

        result += $"└{new string('─', UNIT_WIDTH)}┴{new string('─', UNIT_WIDTH)}┘\n";
        return result;
    }

    /// <summary>
    /// Вывести боевые позиции для стенки на стенку
    /// </summary>
    public string DisplayWallToWallMatchups(IBattleFormation formation, IArmy army1, IArmy army2, string player1Name, string player2Name)
    {
        string result = "\n⚔️ БОЕВЫЕ ЛИНИИ:\n";
        result += "════════════════════════════════════════════════════════════════════════════════════════════════════════\n";

        var allUnits1 = army1.Units.Where(u => u.IsAlive).ToList();
        var allUnits2 = army2.Units.Where(u => u.IsAlive).ToList();

        int maxLen = Math.Max(allUnits1.Count, allUnits2.Count);

        for (int i = 0; i < maxLen; i++)
        {
            string unit1Full = "---";
            string unit2Full = "---";
            string buffLine1 = "";
            string buffLine2 = "";

            if (i < allUnits1.Count)
            {
                var u1 = allUnits1[i];
                string health1 = $"{u1.CurrentHealth}/{u1.MaxHealth}";
                string info1 = GetUnitInfoWithAbilitiesAndBuffs(u1);
                unit1Full = $"{u1.Name} {health1} {info1}";
                buffLine1 = GetBuffInfoLine(u1);
            }

            if (i < allUnits2.Count)
            {
                var u2 = allUnits2[i];
                string health2 = $"{u2.CurrentHealth}/{u2.MaxHealth}";
                string info2 = GetUnitInfoWithAbilitiesAndBuffs(u2);
                unit2Full = $"{u2.Name} {health2} {info2}";
                buffLine2 = GetBuffInfoLine(u2);
            }

            string marker1 = i < allUnits1.Count ? "●" : "○";
            string marker2 = i < allUnits2.Count ? "●" : "○";

            result += $"#{i}: {marker1} {unit1Full,-43} ──── {marker2} #{i}: {unit2Full,-43}\n";
            
            // Добавить баффы на отдельной строке если они есть
            if (!string.IsNullOrEmpty(buffLine1) || !string.IsNullOrEmpty(buffLine2))
            {
                result += $"       {buffLine1,-43} ──── {buffLine2,-43}\n";
            }
        }

        result += "════════════════════════════════════════════════════════════════════════════════════════════════════════\n";
        return result;
    }
}
