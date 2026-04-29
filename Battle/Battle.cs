using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Главный класс для управления боевой системой
/// Отвечает только за логику боя
/// </summary>
public class Battle
{
    private Player _player1;
    private Player _player2;
    private Player _currentPlayer;
    private Player _opponent;
    private int _roundNumber;
    private GameManager _gameManager;
    private BattleDisplayer _displayer;
    private BattleMenuHandler _menuHandler;
    private Random _random;
    private bool _isLoadedGame;
    private bool _autoPlayToEnd;
    private bool _skipFirstTurn;
    private IBattleFormation _formation;
    private int _drawRoundsCounter; // Счетчик ходов без изменений
    private int _noDamageRoundsCounter; // Счетчик ходов без урона
    private Dictionary<int, int> _lastBattleState; // Состояние здоровья юнитов для сравнения
    private List<int> _lastUnitIds; // Состав армий для сравнения

    /// <summary>
    /// Получить обработчик меню боя для проверки статуса сохранений
    /// </summary>
    public BattleMenuHandler MenuHandler => _menuHandler;

    public Battle(Player player1, Player player2, GameManager? gameManager = null)
    {
        ArgumentNullException.ThrowIfNull(player1.Army, nameof(player1.Army));
        ArgumentNullException.ThrowIfNull(player1.Army.Units, nameof(player1.Army));
        ArgumentNullException.ThrowIfNull(player2.Army, nameof(player2.Army));
        ArgumentNullException.ThrowIfNull(player2.Army.Units, nameof(player2.Army));
        
        _player1 = player1;
        _player2 = player2;
        _currentPlayer = player1;
        _opponent = player2;
        _roundNumber = 0;
        _gameManager = gameManager ?? new GameManager();
        _displayer = new BattleDisplayer();
        _menuHandler = new BattleMenuHandler(_gameManager);
        _random = new Random();
        _isLoadedGame = false;
        _autoPlayToEnd = false;
        _skipFirstTurn = false;
        _formation = new BridgeFormation(); // По умолчанию используем боевой мост
        _drawRoundsCounter = 0;
        _lastBattleState = new Dictionary<int, int>();
        _lastUnitIds = new List<int>();
        _noDamageRoundsCounter = 0;
    }

    /// <summary>
    /// Восстановить бой из загруженного состояния
    /// </summary>
    public void SetupLoadedGame(Player currentPlayer, int roundNumber, string gameMode = "BridgeFormation")
    {
        _currentPlayer = currentPlayer;
        _roundNumber = roundNumber;
        _opponent = (_currentPlayer.Name == _player1.Name) ? _player2 : _player1;
        _isLoadedGame = true;
        _skipFirstTurn = true;
        // Устанавливаем боевое построение из загруженного режима
        _formation = GetFormationByName(gameMode);
    }

    /// <summary>
    /// Запустить бой
    /// </summary>
    public Player? Start()
    {
        // Только при новой игре (не при загрузке) случайно определяем начинающего игрока
        if (!_isLoadedGame)
        {
            Random random = new Random();
            if (random.Next(2) == 0)
            {
                _currentPlayer = _player2;
                _opponent = _player1;
            }

            // Выбираем боевое построение перед началом новой битвы
            SelectBattleFormation();
        }

        _displayer.ShowBattleStart(_currentPlayer.Name);

        while (true)
        {
            // Увеличиваем раунд только если это не первая итерация после загрузки
            if (!_isLoadedGame || _roundNumber > 0)
            {
                _roundNumber++;
            }
            else
            {
                // После первого отображения загруженной игры отключаем флаг
                _isLoadedGame = false;
            }
            
            _player1.Army!.RemoveDeadUnits();
            _player2.Army!.RemoveDeadUnits();
            
            int player1Alive = _player1.Army?.Units.Count(u => u.IsAlive) ?? 0;
            int player2Alive = _player2.Army?.Units.Count(u => u.IsAlive) ?? 0;
            
            // Сохраняем состояние боя в начале раунда для отслеживания ничьи
            SaveBattleState();
            int healthSumBefore = _player1.Army!.Units!.Sum(u => u.CurrentHealth) + _player2.Army!.Units!.Sum(u => u.CurrentHealth);
            
            // При авто-проигрывании показываем только номер раунда в одну строку
            if (_autoPlayToEnd)
            {
                Console.WriteLine($"Раунд {_roundNumber}: {_player1.Name} ({player1Alive} юнитов) vs {_player2.Name} ({player2Alive} юнитов)");
            }
            else
            {
                _displayer.ShowRound(_roundNumber, player1Alive, player2Alive, _player1.Name, _player2.Name);
                
                // Визуализация зависит от типа построения
                string armyVis1, armyVis2;
                
                if (_formation is BridgeFormation)
                {
                    armyVis1 = _displayer.DisplayBridgeFormation(_currentPlayer.Army!);
                    armyVis2 = _displayer.DisplayBridgeFormation(_opponent.Army!);
                }
                else if (_formation is ThreeOnThreeFormation)
                {
                    int player1Size = _player1.Army!.Units.Count;
                    bool isCurrentPlayer1 = (_currentPlayer.Army == _player1.Army);
                    armyVis1 = _displayer.DisplayThreeOnThreeFormation(_currentPlayer.Army!, player1Size, isCurrentPlayer1);
                    armyVis2 = _displayer.DisplayThreeOnThreeFormation(_opponent.Army!, player1Size, !isCurrentPlayer1);
                    // Показать формирование
                    Console.WriteLine(armyVis1);
                    Console.WriteLine(armyVis2);
                    // Показать боевые пары
                    Console.WriteLine(_displayer.DisplayThreeOnThreeMatchups(_formation, _currentPlayer.Army!, _opponent.Army!, _currentPlayer.Name, _opponent.Name));
                }
                else if (_formation is WallToWallFormation)
                {
                    armyVis1 = _displayer.DisplayWallToWallFormation(_currentPlayer.Army!);
                    armyVis2 = _displayer.DisplayWallToWallFormation(_opponent.Army!);
                    // Показать боевые линии
                    Console.WriteLine(_displayer.DisplayWallToWallMatchups(_formation, _currentPlayer.Army!, _opponent.Army!, _currentPlayer.Name, _opponent.Name));
                }
                else
                {
                    // Падение назад на стандартную визуализацию
                    armyVis1 = _displayer.GetArmyVisualization(_currentPlayer.Army!);
                    armyVis2 = _displayer.GetArmyVisualization(_opponent.Army!);
                }
                    
                    // Для режима стенка на стенку и 3 на 3 не показываем старые строки здоровья и абилити
                    if (!(_formation is WallToWallFormation) && !(_formation is ThreeOnThreeFormation))
                    {
                        string unitHealth1 = _displayer.GetAllUnitsHealth(_currentPlayer.Army!);
                        string unitAbilities1 = _displayer.GetAllUnitsAbilities(_currentPlayer.Army!);
                        var unitBuffs1 = _displayer.GetAllUnitsBuffs(_currentPlayer.Army!);
                        string unitHealth2 = _displayer.GetAllUnitsHealth(_opponent.Army!);
                        string unitAbilities2 = _displayer.GetAllUnitsAbilities(_opponent.Army!);
                        var unitBuffs2 = _displayer.GetAllUnitsBuffs(_opponent.Army!);
                        _displayer.DisplayArmiesStatus(_currentPlayer.Name, armyVis1, unitHealth1, unitAbilities1, _opponent.Name, armyVis2, unitHealth2, unitAbilities2, unitBuffs1, unitBuffs2);
                    }
            }

            if (!_opponent.Army!.HasAliveUnits())
            {
                var aliveUnits = GetAliveUnitsInfo(_currentPlayer.Army!);
                _displayer.DisplayGameStatistics(_currentPlayer.Name, _roundNumber, aliveUnits);
                return _currentPlayer;
            }

            // Проверяем на ничью: если 10 раундов без изменений или без урона, объявляем ничью
            if (GetDrawCounter() >= 10 || _noDamageRoundsCounter >= 10)
            {
                _displayer.DisplayDrawStatistics(_roundNumber);
                return null;
            }

            // Показываем меню и обрабатываем выбор (только если не в авто-режиме)
            if (!_autoPlayToEnd)
            {
                bool continueRound = HandleMenuChoice(_player1, _player2);
                if (!continueRound)
                {
                    return null;
                }
            }

            var temp = _currentPlayer;
            _currentPlayer = _opponent;
            _opponent = temp;
        }
    }

    /// <summary>
    /// Выполнить один ход
    /// </summary>
    private bool ExecuteTurn(Player currentPlayer, Player opponent)
    {
        var currentArmy = currentPlayer.Army!;
        var opponentArmy = opponent.Army!;

        // Проверяем, есть ли живые юниты
        if (!currentArmy.HasAliveUnits())
        {
            return false;
        }

        // Проверяем, есть ли живые юниты у противника
        if (!opponentArmy.HasAliveUnits())
        {
            return false;
        }

        var action = new AttackAction();
        if (action == null)
        {
            return true;
        }

        // Получаем юнитов, которые находятся на ринге
        var attackersOnRing = _formation.GetRingUnits(currentArmy);
        
        // Каждый боец на ринге атакует своего противника
        foreach (var attacker in attackersOnRing)
        {
            if (!attacker.IsAlive)
                continue;

            var defender = _formation.GetOpponent(attacker, currentArmy, opponentArmy);
            
            if (defender == null || !defender.IsAlive)
                continue;

            int attackerGlobalNum = GetGlobalUnitNumber(attacker, currentArmy);
            int defenderGlobalNum = GetGlobalUnitNumber(defender, opponentArmy);
            action.Execute(attacker, defender, attackerGlobalNum, defenderGlobalNum);

            // После атаки, проверяем потерю баффа у защитника (если он BuffableHeavyInfantry)
            if (defender is IBuffable buffableDefender && buffableDefender.HasBuffs())
            {
                // Небольшая вероятность потерять бафф при попадании
                if (_random.Next(100) < 20) // 20% вероятность
                {
                    if (buffableDefender.RemoveRandomBuff())
                    {
                        int defenderNum = GetGlobalUnitNumber(defender, opponentArmy);
                        _displayer.ShowBuffLost($"{defender.Name} (юнит №{defenderNum})");
                    }
                }
            }

            // Если защитник погиб, обрабатываем замену
            if (!defender.IsAlive)
            {
                int defenderNum = GetGlobalUnitNumber(defender, opponentArmy);
                _displayer.ShowUnitDefeated(defender.Name, "погиб");
            {
                var aliveUnits = GetAliveUnitsInfo(_opponent.Army!);
                _displayer.DisplayGameStatistics(_opponent.Name, _roundNumber, aliveUnits);
                return _opponent;
            }

            _currentPlayer.Army!.RemoveDeadUnits();
            _opponent.Army!.RemoveDeadUnits();
            int healthSumAfter = _player1.Army.Units.Sum(u => u.CurrentHealth) + _player2.Army.Units.Sum(u => u.CurrentHealth);
            if (healthSumAfter < healthSumBefore)
            {
                _noDamageRoundsCounter = 0;
            }
            else
            {
                _noDamageRoundsCounter++;
            }

            // Проверяем, произошли ли изменения после хода первого игрока
            if (HasBattleStateChanged())
            {
                ResetDrawCounter();
            }
            else
            {
                IncrementDrawCounter();
            }
            
            // При авто-проигрывании пропускаем вывод статусов после хода
            if (!_autoPlayToEnd)
            {
                _displayer.ShowStatusUpdate();
                
                // Визуализация зависит от типа построения
                string armyVis1, armyVis2;
                
                if (_formation is BridgeFormation)
                {
                    armyVis1 = _displayer.DisplayBridgeFormation(_currentPlayer.Army!);
                    armyVis2 = _displayer.DisplayBridgeFormation(_opponent.Army!);
                }
                else if (_formation is ThreeOnThreeFormation)
                {
                    int player1Size = _player1.Army!.Units.Count;
                    bool isCurrentPlayer1 = (_currentPlayer.Army == _player1.Army);
                    armyVis1 = _displayer.DisplayThreeOnThreeFormation(_currentPlayer.Army!, player1Size, isCurrentPlayer1);
                    armyVis2 = _displayer.DisplayThreeOnThreeFormation(_opponent.Army!, player1Size, !isCurrentPlayer1);
                    // Показать формирование
                    Console.WriteLine(armyVis1);
                    Console.WriteLine(armyVis2);
                    // Показать боевые пары
                    Console.WriteLine(_displayer.DisplayThreeOnThreeMatchups(_formation, _currentPlayer.Army!, _opponent.Army!, _currentPlayer.Name, _opponent.Name));
                }
                else if (_formation is WallToWallFormation)
                {
                    armyVis1 = _displayer.DisplayWallToWallFormation(_currentPlayer.Army!);
                    armyVis2 = _displayer.DisplayWallToWallFormation(_opponent.Army!);
                    // Показать боевые линии
                    Console.WriteLine(_displayer.DisplayWallToWallMatchups(_formation, _currentPlayer.Army!, _opponent.Army!, _currentPlayer.Name, _opponent.Name));
                }
                else
                {
                    // Падение назад на стандартную визуализацию
                    armyVis1 = _displayer.GetArmyVisualization(_currentPlayer.Army!);
                    armyVis2 = _displayer.GetArmyVisualization(_opponent.Army!);
                }
                
                // Для режима стенка на стенку и 3 на 3 не показываем старые строки здоровья и абилити
                if (!(_formation is WallToWallFormation) && !(_formation is ThreeOnThreeFormation))
                {
                    string unitHealth1 = _displayer.GetAllUnitsHealth(_currentPlayer.Army!);
                    string unitAbilities1 = _displayer.GetAllUnitsAbilities(_currentPlayer.Army!);
                    var unitBuffs1 = _displayer.GetAllUnitsBuffs(_currentPlayer.Army!);
                    string unitHealth2 = _displayer.GetAllUnitsHealth(_opponent.Army!);
                    string unitAbilities2 = _displayer.GetAllUnitsAbilities(_opponent.Army!);
                    var unitBuffs2 = _displayer.GetAllUnitsBuffs(_opponent.Army!);
                    _displayer.DisplayArmiesStatus(_currentPlayer.Name, armyVis1, unitHealth1, unitAbilities1, _opponent.Name, armyVis2, unitHealth2, unitAbilities2, unitBuffs1, unitBuffs2);
                }
            }

            if (!_opponent.Army!.HasAliveUnits())
            {
                var aliveUnits = GetAliveUnitsInfo(_currentPlayer.Army!);
                _displayer.DisplayGameStatistics(_currentPlayer.Name, _roundNumber, aliveUnits);
                return _currentPlayer;
            }

            // Проверяем на ничью: если 10 раундов без изменений или без урона, объявляем ничью
            if (GetDrawCounter() >= 10 || _noDamageRoundsCounter >= 10)
            {
                _displayer.DisplayDrawStatistics(_roundNumber);
                return null;
            }

            // Показываем меню и обрабатываем выбор (только если не в авто-режиме)
            if (!_autoPlayToEnd)
            {
                bool continueRound = HandleMenuChoice(_player1, _player2);
                if (!continueRound)
                {
                    return null;
                }
            }

            var temp = _currentPlayer;
            _currentPlayer = _opponent;
            _opponent = temp;
        }
    }

    /// <summary>
    /// Выполнить один ход
    /// </summary>
    private bool ExecuteTurn(Player currentPlayer, Player opponent)
    {
        var currentArmy = currentPlayer.Army!;
        var opponentArmy = opponent.Army!;

        // Проверяем, есть ли живые юниты
        if (!currentArmy.HasAliveUnits())
        {
            return false;
        }

        // Проверяем, есть ли живые юниты у противника
        if (!opponentArmy.HasAliveUnits())
        {
            return false;
        }

        var action = new AttackAction();
        if (action == null)
        {
            return true;
        }

        // Получаем юнитов, которые находятся на ринге
        var attackersOnRing = _formation.GetRingUnits(currentArmy);
        
        // Каждый боец на ринге атакует своего противника
        foreach (var attacker in attackersOnRing)
        {
            if (!attacker.IsAlive)
                continue;

            var defender = _formation.GetOpponent(attacker, currentArmy, opponentArmy);
            
            if (defender == null || !defender.IsAlive)
                continue;

            int attackerGlobalNum = GetGlobalUnitNumber(attacker, currentArmy);
            int defenderGlobalNum = GetGlobalUnitNumber(defender, opponentArmy);
            action.Execute(attacker, defender, attackerGlobalNum, defenderGlobalNum);

            // После атаки, проверяем потерю баффа у защитника (если он BuffableHeavyInfantry)
            if (defender is IBuffable buffableDefender && buffableDefender.HasBuffs())
            {
                // Небольшая вероятность потерять бафф при попадании
                if (_random.Next(100) < 20) // 20% вероятность
                {
                    if (buffableDefender.RemoveRandomBuff())
                    {
                        int defenderNum = GetGlobalUnitNumber(defender, opponentArmy);
                        _displayer.ShowBuffLost($"{defender.Name} (юнит №{defenderNum})");
                    }
                }
            }

            // Если защитник погиб, обрабатываем замену
            if (!defender.IsAlive)
            {
                int defenderNum = GetGlobalUnitNumber(defender, opponentArmy);
                _displayer.ShowUnitDefeated(defender.Name, "погиб");
                _formation.HandleUnitDeath(defender, opponentArmy, currentArmy);
                
                // Проверяем, есть ли живые юниты
                if (!opponentArmy.HasAliveUnits())
                {
                    return true;
                }
                
                // Показываем следующего юнита (если он есть)
                var nextUnit = _formation.GetRingUnits(opponentArmy).FirstOrDefault();
                if (nextUnit != null)
                {
                    _displayer.ShowNewUnitAppears(nextUnit.Name);
                }
            }
        }

        ExecuteArcherAttack(currentArmy, opponentArmy);
        ExecuteWizardClone(currentArmy);
        ExecuteHealerAttack(currentArmy);
        ExecuteSquireAbility(currentArmy);

        return true;
    }

    /// <summary>
    /// Выполнить атаку всех лучников из армии
    /// </summary>
    private void ExecuteArcherAttack(IArmy currentArmy, IArmy opponentArmy)
    {
        var archers = currentArmy.Units
            .Where(u => u.IsAlive && u.SpecialAbility is ArrowAbility)
            .ToList();
        
        foreach (var archer in archers)
        {
            int archerPosition = currentArmy.Units.IndexOf(archer) + 1;

            if (archerPosition == 1)
            {
                int archerGlobalNum = GetGlobalUnitNumber(archer, currentArmy);
                _displayer.ShowArcherCannotShootFirst(archer.Name, archerGlobalNum);
                continue;
            }

            // В режиме стенка на стенку лучник не использует способность, если у него есть противник
            if (_formation is WallToWallFormation wallFormation && wallFormation.HasOpponent(archer, currentArmy, opponentArmy))
            {
                continue;
            }

            if (archer.SpecialAbility is ArrowAbility arrowAbility)
            {
                arrowAbility.SetUnitPosition(archerPosition - 1);
                arrowAbility.SetArcher(archer);

                var availableTargets = arrowAbility.GetAvailableTargets(opponentArmy, _formation, currentArmy);

                if (availableTargets.Count == 0)
                {
                    int archerGlobalNum = GetGlobalUnitNumber(archer, currentArmy);
                    _displayer.ShowArcherNoTargets(archer.Name, archerGlobalNum, arrowAbility.Range);
                    continue;
                }

                var targetToAttack = availableTargets[_random.Next(availableTargets.Count)];

                int healthBefore = targetToAttack.CurrentHealth;
                arrowAbility.Activate(targetToAttack);
                int damageDealt = healthBefore - targetToAttack.CurrentHealth;

                int archerGlobalNumber = GetGlobalUnitNumber(archer, currentArmy);
                int targetGlobalNumber = GetGlobalUnitNumber(targetToAttack, opponentArmy);
                _displayer.ShowArcherAttackResult(archer.Name, archerGlobalNumber, targetToAttack.Name, targetGlobalNumber, damageDealt, 
                                                   targetToAttack.CurrentHealth, targetToAttack.MaxHealth);

                if (!targetToAttack.IsAlive)
                {
                    int targetGlobalNum = GetGlobalUnitNumber(targetToAttack, opponentArmy);
                    _displayer.ShowUnitDefeated($"{targetToAttack.Name} (юнит №{targetGlobalNum})", "погиб от стрелы");
                    opponentArmy.RemoveDeadUnits();

                    if (opponentArmy.HasAliveUnits())
                    {
                        var nextUnit = opponentArmy.GetCurrentUnit();
                        int nextGlobalNum = GetGlobalUnitNumber(nextUnit!, opponentArmy);
                        _displayer.ShowNewUnitAppears($"{nextUnit?.Name} (юнит №{nextGlobalNum})");
                    }
                }
            }
        }
    }

    /// <summary>
    /// Получить информацию об оставшихся живых юнитах
    /// </summary>
    private List<(string Name, int CurrentHealth, int MaxHealth)> GetAliveUnitsInfo(IArmy army)
    {
        var aliveUnits = new List<(string, int, int)>();
        foreach (var unit in army.Units)
        {
            if (unit.IsAlive)
            {
                aliveUnits.Add((unit.Name, unit.CurrentHealth, unit.MaxHealth));
            }
        }
        return aliveUnits;
    }

    /// <summary>
    /// Выполнить лечение всех лекарей из армии
    /// </summary>
    private void ExecuteHealerAttack(IArmy currentArmy)
    {
        var healers = currentArmy.Units
            .Where(u => u.IsAlive && u.SpecialAbility is HealAbility)
            .ToList();
        
        foreach (var healer in healers)
        {
            int healerPosition = currentArmy.Units.IndexOf(healer) + 1;

            if (healerPosition == 1)
                continue;

            // В режиме стенка на стенку лекарь не использует способность, если у него есть противник
            if (_formation is WallToWallFormation wallFormation && wallFormation.HasOpponent(healer, currentArmy, _player1.Army! == currentArmy ? _player2.Army! : _player1.Army!))
            {
                continue;
            }

            if (healer.SpecialAbility is HealAbility healAbility)
            {
                healAbility.SetUnitPosition(healerPosition - 1);

                healAbility.SetHealer(healer);
                var availableTargets = healAbility.GetAvailableTargets(currentArmy, _formation);

                if (availableTargets.Count == 0)
                {
                    int healerGlobalNum = GetGlobalUnitNumber(healer, currentArmy);
                    _displayer.ShowHealerNoTargets(healer.Name, healerGlobalNum);
                    continue;
                }

                var targetToHeal = availableTargets[_random.Next(availableTargets.Count)];

                int healthBefore = targetToHeal.CurrentHealth;
                healAbility.Activate(targetToHeal);
                int healAmount = targetToHeal.CurrentHealth - healthBefore;

                int targetGlobalNum = GetGlobalUnitNumber(targetToHeal, currentArmy);
                _displayer.ShowHealerAttackResult(targetToHeal.Name, targetGlobalNum, healAmount, 
                                                  targetToHeal.CurrentHealth, targetToHeal.MaxHealth);
            }
        }
    }

    /// <summary>
    /// Выполнить клонирование всеми волшебниками из армии
    /// </summary>
    private void ExecuteWizardClone(IArmy currentArmy)
    {
        var wizards = currentArmy.Units
            .Where(u => u.IsAlive && u.SpecialAbility is CloneAbility)
            .ToList();
        
        foreach (var wizard in wizards)
        {
            int wizardPosition = currentArmy.Units.IndexOf(wizard) + 1;

            if (wizardPosition == 1)
                continue;

            // В режиме стенка на стенку волшебник не использует способность, если у него есть противник
            if (_formation is WallToWallFormation wallFormation && wallFormation.HasOpponent(wizard, currentArmy, _player1.Army! == currentArmy ? _player2.Army! : _player1.Army!))
            {
                continue;
            }

            if (wizard.SpecialAbility is CloneAbility cloneAbility)
            {
                cloneAbility.SetUnitPosition(wizardPosition - 1);

                cloneAbility.SetWizard(wizard);
                var availableTargets = cloneAbility.GetAvailableTargets(currentArmy, _formation);

                if (availableTargets.Count == 0)
                {
                    int wizardGlobalNum = GetGlobalUnitNumber(wizard, currentArmy);
                    _displayer.ShowWizardNoTargets(wizard.Name, wizardGlobalNum);
                    continue;
                }

                var targetToClone = availableTargets[_random.Next(availableTargets.Count)];

                int wizardGlobalNumber = GetGlobalUnitNumber(wizard, currentArmy);
                int targetGlobalNumber = GetGlobalUnitNumber(targetToClone, currentArmy);
                _displayer.ShowWizardCloneAttempt(wizard.Name, wizardGlobalNumber, targetToClone.Name, cloneAbility.CloneProbability);

                // Пытаемся клонировать (проверяем вероятность)
                bool cloneSuccess = cloneAbility.TryClone();
                
                if (cloneSuccess)
                {
                    // Создаем клона
                    var clone = cloneAbility.CreateClone(targetToClone);
                    
                    if (clone != null)
                    {
                        // Вставляем клона ПЕРЕД магом
                        currentArmy.Units.Insert(wizardPosition - 1, clone);
                        int targetGlobalNum = GetGlobalUnitNumber(targetToClone, currentArmy);
                        _displayer.ShowWizardCloneSuccess(targetToClone.Name, targetGlobalNum, clone.Name);
                    }
                    else
                    {
                        int targetGlobalNum = GetGlobalUnitNumber(targetToClone, currentArmy);
                        _displayer.ShowWizardCloneFailed(targetToClone.Name, targetGlobalNum, cloneAbility.CloneProbability);
                    }
                }
                else
                {
                    int targetGlobalNum = GetGlobalUnitNumber(targetToClone, currentArmy);
                    _displayer.ShowWizardCloneFailed(targetToClone.Name, targetGlobalNum, cloneAbility.CloneProbability);
                }
            }
        }
    }

    /// <summary>
    /// Выполнить применение баффов всеми оруженосцами из армии
    /// </summary>
    private void ExecuteSquireAbility(IArmy currentArmy)
    {
        var squires = currentArmy.Units
            .Where(u => u.IsAlive && u.SpecialAbility is SquireAbility)
            .ToList();
        
        foreach (var squire in squires)
        {
            int squirePosition = currentArmy.Units.IndexOf(squire) + 1;

            if (squirePosition == 1)
                continue;

            // В режиме стенка на стенку оруженосец не использует способность, если у него есть противник
            if (_formation is WallToWallFormation wallFormation && wallFormation.HasOpponent(squire, currentArmy, _player1.Army! == currentArmy ? _player2.Army! : _player1.Army!))
            {
                continue;
            }

            if (squire.SpecialAbility is SquireAbility squireAbility)
            {
                squireAbility.SetUnitPosition(squirePosition - 1);
                squireAbility.SetSquire(squire);

                var availableTargets = squireAbility.GetAvailableTargets(currentArmy, _formation);

                if (availableTargets.Count == 0)
                {
                    int squireGlobalNum = GetGlobalUnitNumber(squire, currentArmy);
                    _displayer.ShowSquireNoTargets(squire.Name, squireGlobalNum);
                    continue;
                }

                // Пытаемся активировать способность
                bool abilitySuccess = squireAbility.TryActivate();
                
                if (abilitySuccess)
                {
                    // Выбираем цель - если несколько, берем с большим HP
                    IUnit targetUnit = availableTargets[0];
                    if (availableTargets.Count > 1)
                    {
                        targetUnit = availableTargets[0].CurrentHealth >= availableTargets[1].CurrentHealth 
                            ? availableTargets[0] 
                            : availableTargets[1];
                    }

                    int squireGlobalNumber = GetGlobalUnitNumber(squire, currentArmy);
                    int targetGlobalNumber = GetGlobalUnitNumber(targetUnit, currentArmy);
                    _displayer.ShowSquireBuffAttempt(squire.Name, squireGlobalNumber, targetUnit.Name, targetGlobalNumber, squireAbility.ActivationProbability);

                    // Применяем бафф
                    if (targetUnit is IBuffable buffableTarget)
                    {
                        squireAbility.Activate(buffableTarget);
                        var buffs = buffableTarget.GetActiveBuffs();
                        var lastBuff = buffs[buffs.Count - 1];
                        int squireGlobalNum = GetGlobalUnitNumber(squire, currentArmy);
                        int targetGlobalNum = GetGlobalUnitNumber(targetUnit, currentArmy);
                        _displayer.ShowSquireBuffSuccess(squire.Name, squireGlobalNum, targetUnit.Name, targetGlobalNum, lastBuff.Name);
                    }
                }
                else
                {
                    int squireGlobalNum = GetGlobalUnitNumber(squire, currentArmy);
                    _displayer.ShowSquireBuffFailed(squire.Name, squireGlobalNum, squireAbility.ActivationProbability);
                }
            }
        }
    }

    /// <summary>
    /// Обработать выбор в меню между раундами
    /// </summary>
    private bool HandleMenuChoice(Player player1, Player player2)
    {
        while (true)
        {
            var menuChoice = _menuHandler.ShowRoundMenu(player1, player2, _currentPlayer, _roundNumber, GetFormationName());
            
            switch (menuChoice)
            {
                case BattleMenuHandler.RoundMenuChoice.Continue:
                    return true;
                    
                case BattleMenuHandler.RoundMenuChoice.AutoPlayToEnd:
                    _autoPlayToEnd = true;
                    return true;
                    
                case BattleMenuHandler.RoundMenuChoice.ChangeFormation:
                    _formation = _menuHandler.ChooseFormation();
                    // Показываем армии с новым форматом
                    if (_formation is BridgeFormation)
                    {
                        Console.WriteLine(_displayer.DisplayBridgeFormation(_currentPlayer.Army!));
                        Console.WriteLine(_displayer.DisplayBridgeFormation(_opponent.Army!));
                    }
                    else if (_formation is ThreeOnThreeFormation)
                    {
                        int player1Size = _player1.Army!.Units.Count;
                        bool isCurrentPlayer1 = (_currentPlayer.Army == _player1.Army);
                        Console.WriteLine(_displayer.DisplayThreeOnThreeFormation(_currentPlayer.Army!, player1Size, isCurrentPlayer1));
                        Console.WriteLine(_displayer.DisplayThreeOnThreeFormation(_opponent.Army!, player1Size, !isCurrentPlayer1));
                    }
                    else if (_formation is WallToWallFormation)
                    {
                        Console.WriteLine(_displayer.DisplayWallToWallFormation(_currentPlayer.Army!));
                        Console.WriteLine(_displayer.DisplayWallToWallFormation(_opponent.Army!));
                    }
                    continue; // Показываем меню снова
                    
                case BattleMenuHandler.RoundMenuChoice.Exit:
                    return false;
                    
                default:
                    return true;
            }
        }
    }

    /// <summary>
    /// Выбрать боевое построение перед началом боя
    /// </summary>
    private void SelectBattleFormation()
    {
        var selector = new BattleFormationSelector();
        _formation = selector.SelectFormation();
        Console.WriteLine($"\nВыбрано построение: {_formation.Name}\n");
    }

    /// <summary>
    /// Сохранить текущее состояние здоровья всех юнитов обеих армий
    /// </summary>
    private void SaveBattleState()
    {
        _lastBattleState.Clear();
        _lastUnitIds.Clear();
        // Сохраняем состояние для каждого юнита в виде ID -> CurrentHealth
        foreach (var unit in _player1.Army!.Units.Concat(_player2.Army!.Units))
        {
            _lastBattleState[unit.Id] = unit.CurrentHealth;
            _lastUnitIds.Add(unit.Id);
        }
    }

    /// <summary>
    /// Проверить, изменилось ли состояние боя (здоровье юнитов)
    /// </summary>
    private bool HasBattleStateChanged()
    {
        var allUnits = _player1.Army!.Units.Concat(_player2.Army!.Units).ToList();
        // Проверяем состав армий (id юнитов)
        if (allUnits.Count != _lastUnitIds.Count)
            return true;
        for (int i = 0; i < allUnits.Count; i++)
        {
            if (allUnits[i].Id != _lastUnitIds[i])
                return true;
        }
        // Проверяем здоровье
        foreach (var unit in allUnits)
        {
            if (!_lastBattleState.ContainsKey(unit.Id) || _lastBattleState[unit.Id] != unit.CurrentHealth)
            {
                return true; // Состояние изменилось
            }
        }
        return false; // Состояние не изменилось
    }

    /// <summary>
    /// Обнулить счетчик ходов без изменений
    /// </summary>
    private void ResetDrawCounter()
    {
        _drawRoundsCounter = 0;
    }

    /// <summary>
    /// Увеличить счетчик ходов без изменений
    /// </summary>
    private void IncrementDrawCounter()
    {
        _drawRoundsCounter++;
    }

    /// <summary>
    /// Получить текущее значение счетчика ходов без изменений
    /// </summary>
    private int GetDrawCounter()
    {
        return _drawRoundsCounter;
    }

    /// <summary>
    /// Получить боевое построение по названию класса
    /// </summary>
    private IBattleFormation GetFormationByName(string formationName)
    {
        return formationName switch
        {
            "ThreeOnThreeFormation" => new ThreeOnThreeFormation(),
            "WallToWallFormation" => new WallToWallFormation(),
            _ => new BridgeFormation() // По умолчанию
        };
    }

    /// <summary>
    /// Получить название класса текущего боевого построения
    /// </summary>
    private string GetFormationName()
    {
        return _formation.GetType().Name;
    }

    /// <summary>
    /// Получить глобальный номер юнита (с учетом обеих армий)
    /// Первая армия: 1..N, вторая армия: N+1..N+M
    /// </summary>
    private int GetGlobalUnitNumber(IUnit unit, IArmy unitArmy)
    {
        int localIndex = unitArmy.Units.IndexOf(unit);
        int player1Count = _player1.Army!.Units.Count;
        
        if (unitArmy == _player1.Army)
        {
            return localIndex + 1; // 1..N
        }
        else
        {
            return player1Count + localIndex + 1; // N+1..N+M
        }
    }
}
