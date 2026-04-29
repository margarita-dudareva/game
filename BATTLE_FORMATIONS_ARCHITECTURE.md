# Архитектурный план: Система боевых построений

## 📋 Общая структура

Используем паттерн **Strategy** для управления тремя режимами боя:
1. **BridgeFormation** - боевой мост (текущий, 1 на 1)
2. **ThreeOnThreeFormation** - 3 на 3 одновременно
3. **WallToWallFormation** - стенка на стенку (все в ряд)

---

## 🏗️ Архитектура классов

### 1. **IBattleFormation** (интерфейс Strategy)
```csharp
public interface IBattleFormation
{
    /// Название построения
    string Name { get; }
    
    /// Получить юнитов, находящихся "на ринге" (активные бойцы)
    List<IUnit> GetRingUnits(IArmy army);
    
    /// Получить юнитов в "запасе" (не на ринге, могут использовать спецсособности)
    List<IUnit> GetReserveUnits(IArmy army);
    
    /// Проверить, находится ли юнит на ринге
    bool IsOnRing(IUnit unit, IArmy army);
    
    /// Получить противника для юнита (его соусок на ринге)
    IUnit? GetOpponent(IUnit unit, IArmy army, IArmy opponentArmy);
    
    /// Получить соседей юнита (для способностей типа "стоит рядом")
    List<IUnit> GetAdjacentUnits(IUnit unit, IArmy army);
    
    /// Обработать смерть юнита (замены, перестройка армии)
    void HandleUnitDeath(IUnit deadUnit, IArmy army, IArmy opponentArmy);
    
    /// Переместить юнита на ринг (если запас закончился)
    void PromoteUnitToRing(IUnit unit, IArmy army);
    
    /// Получить позицию юнита в строю (для ренджа спецсособностей)
    (int row, int column) GetUnitPosition(IUnit unit, IArmy army);
    
    /// Получить юнитов в радиусе (для круговой атаки в 3на3/стенка)
    List<IUnit> GetUnitsInRadius(IUnit unit, IArmy army, int radius);
}
```

---

## 📐 Подробное описание каждого режима

### **BridgeFormation** (1 на 1)
```
ТЕКУЩИЙ:
Армия 1: 5 4 3 2 1 0 (очередь)
Армия 2: 0 1 2 3 4 5 6 7 (очередь)

На ринге: только первый юнит каждой армии (индекс 0)
В запасе: остальные (индеки 1+)
Смерть юнита: следующий юнит из очереди (индекс 0 → 1)
Соседи: нет (каждый юнит один)
```

**Реализация:**
- `GetRingUnits()` → первый живой юнит
- `GetReserveUnits()` → остальные живые юниты
- `GetAdjacentUnits()` → пусто
- При смерти: юнит с индексом 1 становится индексом 0

---

### **ThreeOnThreeFormation** (3 на 3)
```
ТРОЙНОЙ РЯД:

ринг      запас
4         0   против   0    3  6
5    →    1   против   1    4  7
          2   против   2    5

Структура:
- Армия делится на 3 колонки (в идеале)
- Первая колонка - НА РИНГЕ (3 юнита сражаются)
- Вторая и третья - В ЗАПАСЕ
- Если юнитов < 3: колонки могут быть неполными

На ринге: юниты с индексами [0, 1, 2]
Соседи: 
  - Юнит на позиции (0,0) - соседи (1,0), (0,1)
  - Юнит на позиции (0,1) - соседи (1,1), (0,0), (0,2)
  - Юнит на позиции (0,2) - соседи (1,2), (0,1)
  + диагонали и все соседние ячейки (круговой рендж)

Смерть юнита на ринге:
  1. Берём первого живого из запаса (индекс 3)
  2. Переносим его на место мёртвого (индекс 0→0, но это следующий юнит)
  3. Остальные в запасе сдвигаются на место перемещённого
  ИЛИ: просто удаляем мёртвого, остальные заполняют позицию автоматически
```

**Реализация:**
```
Позиция вычисляется как:
row = index / 3
column = index % 3

На ринге: row == 0, column == 0,1,2
В запасе: row > 0
```

---

### **WallToWallFormation** (Стенка на стенку)
```
ФРОНТАЛЬНЫЙ РЯД:

0  против  0
1  против  1
2  против  2
3  против  3
4  против  4
5  против  5
           6 - нет противника (в запасе)
           7 - нет противника (в запасе)

На ринге: юниты для которых есть противник
В запасе: юниты без противника (если одна армия больше)

Соседи:
- Юнит на позиции 0: сосед 1 (снизу)
- Юнит на позиции 1: соседи 0 (сверху), 2 (снизу)
- Юнит на позиции 5: сосед 4 (сверху)
+ диагонали

ПРОБЛЕМА ДЫРЫ: если юнит на ринге умирает → не может быть дыры
Решение: переместить живых вверх (схлопнуть предыдущих)
```

**Реализация:**
```
На ринге: min(army1.AliveCount, army2.AliveCount) юнитов
В запасе: |army1.AliveCount - army2.AliveCount| юнитов

Позиция = индекс в армии

Смерть юнита на ринге:
1. Юнит с индексом 5 удален
2. Юнит 4 остаётся на месте (он под новым номером сверху)
3. НЕТ ДЫРЫ, потому что оставшиеся юниты заполняют пробел вверх
```

---

## 🔄 Обработка смертей и замен

### Общий алгоритм для всех режимов:

```
1. Юнит умирает
2. Вызываем `formation.HandleUnitDeath(deadUnit, army, opponentArmy)`
3. В зависимости от режима:

   [BridgeFormation]:
   - Юнит на ринге умирает → следующий из запаса автоматически становится текущим
   
   [ThreeOnThreeFormation]:
   - Юнит на ринге (row==0) умирает
   - Если есть живые в запасе (row>0) → перемещаем первого в его позицию
   - Здесь может быть логика "поднятия" рядов
   
   [WallToWallFormation]:
   - Юнит умирает, остальные "схлопываются" вверх
   - Новый последний юнит опускается в запас (если было неравенство)
```

---

## 🎯 Соседства для спецсособностей

### Для BridgeFormation:
- Нет соседей (каждый один)
- Способности типа "рядом" не срабатывают

### Для ThreeOnThreeFormation и WallToWallFormation:
- **Соседи** = все окружающие ячейки (фронт, обе стороны, тыл, диагонали)
- **Тип ренджа**: КРУГОВОЙ
- **Расстояние**: 1 = соседи, 2 = через одного и т.д.

---

## 💾 Структура данных для армии

Текущая структура в `Army.cs`:
```csharp
public List<IUnit> Units { get; private set; } = new();
```

**Не нужно менять** - просто используем индексы по-разному в зависимости от формации.

---

## 🎮 Интеграция в Battle.cs

### Текущий поток боя:
```
Battle.Start()
  → SelectBattleFormation() [НОВОЕ - меню выбора]
  → _formation = selectedFormation
  → do {
      ExecuteTurn()
    } while (battle continues)
```

### Метод DoRound():
```
БЫЛО:
- attacker = currentArmy.GetCurrentUnit()
- defender = opponentArmy.GetCurrentUnit()
- Attack
- Switch turns

БУДЕТ:
- attackers = _formation.GetRingUnits(currentArmy)
- для каждого attacker:
    - defender = _formation.GetOpponent(attacker, currentArmy, opponentArmy)
    - if (defender != null) Attack(attacker, defender)
    - if (defender.Dead) _formation.HandleUnitDeath(defender, opponentArmy, currentArmy)

- reserveUnits1 = _formation.GetReserveUnits(players[0].Army)
- reserveUnits2 = _formation.GetReserveUnits(players[1].Army)
- Спецсособности для резервных юнитов обеих армий
```

---

## 📁 Новые файлы

```
Battle/
  IBattleFormation.cs          — интерфейс Strategy
  BridgeFormation.cs           — текущий режим (1 на 1)
  ThreeOnThreeFormation.cs     — режим 3 на 3
  WallToWallFormation.cs       — режим стенка на стенку
  BattleFormationSelector.cs   — меню выбора режима

(Можно поместить в подпапку Battle/Formations/)
```

---

## 🔧 Реализация поэтапно

### Этап 1: База
- [ ] Создать `IBattleFormation.cs`
- [ ] Создать `BridgeFormation.cs` (перенести текущую логику)
- [ ] Создать `BattleFormationSelector.cs` (меню внутри Battle)

### Этап 2: Интеграция
- [ ] Модифицировать `Battle.cs` для использования `_formation`
- [ ] Обновить `DoRound()` для работы с массивом юнитов на ринге
- [ ] Обновить спецсособности для использования `GetAdjacentUnits()`

### Этап 3: ThreeOnThreeFormation
- [ ] Реализовать логику при делении на 3
- [ ] Обработка замен
- [ ] Круговой рендж

### Этап 4: WallToWallFormation
- [ ] Реализовать логику при выстраивании в ряд
- [ ] Обработка дыр (схлопывание)
- [ ] Круговой рендж

### Этап 5: Тестирование
- [ ] Протестировать каждый режим
- [ ] Проверить обработку смертей
- [ ] Проверить спецсособности

---

## ⚠️ Критические моменты

1. **Индексы**: 
   - BridgeFormation: используется FirstOrDefault (всегда берём первого живого)
   - Остальные: используются индексы и позиции

2. **Спецсособности**:
   - BridgeFormation: ничего не меняется (нет соседей)
   - ThreeOnThreeFormation: соседи = окружающие + диагонали
   - WallToWallFormation: соседи = сверху/снизу + диагонали

3. **Смерти**:
   - Критично правильно обрабатывать замены
   - Нельзя оставлять дыры на ринге в WallToWallFormation

4. **Отображение**:
   - BattleDisplayer нужно обновить для показа разных построений
   - Может потребоваться новый метод `DisplayFormation()` для каждого режима

---

## 🎬 Примеры использования после реализации

```csharp
// Создание
var formation = new ThreeOnThreeFormation();
var battle = new Battle(player1, player2);
battle.SetFormation(formation);

// Проверка позиций
foreach (var unit in army.Units)
{
    bool onRing = formation.IsOnRing(unit, army);
    var adjacent = formation.GetAdjacentUnits(unit, army);
    var opponent = formation.GetOpponent(unit, army, opponentArmy);
}

// Обработка смерти
formation.HandleUnitDeath(deadUnit, army, opponentArmy);
```

---

## 📝 Примечания

- **Паттерн Strategy** позволит легко добавлять новые режимы в будущем
- **Не меняем структуру Army** - работаем с индексами по-разному
- **Спецсособности** автоматически адаптируются через `GetAdjacentUnits()`
- **Отображение** может быть реализовано после базовой логики или параллельно
