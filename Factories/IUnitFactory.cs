/// <summary>
/// Интерфейс для фабрики создания юнитов
/// Позволяет абстрагироваться от конкретных типов юнитов
/// </summary>
public interface IUnitFactory
{
    /// <summary>
    /// Создать и вернуть юнита
    /// </summary>
    IUnit CreateUnit();
    
    /// <summary>
    /// Название типа юнита
    /// </summary>
    string UnitTypeName { get; }
}
