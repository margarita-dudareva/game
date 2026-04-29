using System;

/// <summary>
/// Управление процессом создания армии (ручное или случайное)
/// Отделена от потока игры и отображения
/// </summary>
public class ArmyCreationManager
{
    private InputValidator _inputValidator;
    private ArmyBuilder _armyBuilder;

    public ArmyCreationManager(InputValidator? inputValidator = null)
    {
        _inputValidator = inputValidator ?? new InputValidator();
        _armyBuilder = new ArmyBuilder();
    }

    /// <summary>
    /// Создать армию для игрока (ручное или случайное создание)
    /// </summary>
    public Army CreateArmy(string playerName, int budget, bool isManual)
    {
        if (isManual)
        {
            return CreateArmyManual(playerName, budget);
        }
        else
        {
            return CreateArmyRandom(playerName, budget);
        }
    }

    /// <summary>
    /// Создать армию случайно
    /// </summary>
    private Army CreateArmyRandom(string name, int budget)
    {
        var factories = _armyBuilder.GetDefaultUnitFactories();
        return _armyBuilder.CreateRandomArmyWithFactories(name, budget, factories);
    }

    /// <summary>
    /// Создать армию вручную
    /// </summary>
    private Army CreateArmyManual(string playerName, int budget)
    {
        var army = new Army(playerName + " Army");
        int remainingBudget = budget;
        var pool = _armyBuilder.GenerateUnitOptions();

        while (true)
        {
            Console.WriteLine($"\nБюджет: {remainingBudget}");

            Console.WriteLine("\nДоступные юниты:");
            _armyBuilder.PrintUnitTable(pool);

            Console.WriteLine($"\n{playerName}, ваша армия:");
            if (army.Units.Count > 0)
            {
                _armyBuilder.PrintUnitTable(army.Units);
            }
            else
            {
                Console.WriteLine("Армия пуста");
            }

            Console.WriteLine("\nВведите номер юнита чтобы добавить");
            Console.WriteLine("Введите -1 чтобы удалить юнита");
            Console.WriteLine("Введите 0 чтобы завершить");

            var choice = _inputValidator.GetManualArmyChoice(pool.Count);

            if (choice == 0)
            {
                break;
            }

            if (choice == -1)
            {
                if (army.Units.Count == 0)
                {
                    Console.WriteLine("Армия пуста");
                    continue;
                }

                Console.WriteLine("Введите номер юнита для удаления:");
                var removeInput = _inputValidator.GetManualArmyChoice(army.Units.Count);

                if (removeInput > 0 && removeInput <= army.Units.Count)
                {
                    var removedUnit = army.Units[removeInput - 1];
                    army.RemoveUnit(removedUnit);
                    // Вернуть юнит в пул (не создаём копию, используем удалённый)
                    pool.Add(removedUnit);
                    remainingBudget += removedUnit.Cost;
                }

                continue;
            }

            if (choice > 0 && choice <= pool.Count)
            {
                var selected = pool[choice - 1];

                if (remainingBudget < selected.Cost)
                {
                    Console.WriteLine("Недостаточно средств!");
                }
                else
                {
                    // Создать новый юнит с правильным ID вместо использования опции напрямую
                    var newUnit = _armyBuilder.DuplicateUnitWithNewId(selected);
                    army.AddUnit(newUnit);
                    remainingBudget -= selected.Cost;
                    pool.RemoveAt(choice - 1);
                }
            }
        }

        return army;
    }
}
