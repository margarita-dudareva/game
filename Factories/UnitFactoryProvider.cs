using System.Collections.Generic;

public class UnitFactoryProvider : IUnitFactoryProvider
{
    public List<IUnitFactory> GetUnitFactories()
    {
        var result = new List<IUnitFactory>
        {
            new LightInfantryFactory(),
            new HeavyInfantryFactory(),
            new ArcherFactory(),
            new HealerFactory(),
            new WizardFactory(),
            new GulyayGorodAdapterFactory()
        };

        if (!ProxySettings.Current.EnableDamageLogging && !ProxySettings.Current.EnableDeathBeep)
            return result;

        var observed = new List<IUnitFactory>();
        foreach (var factory in result)
        {
            observed.Add(new ObserverUnitFactory(factory));
        }

        return observed;
    }
}

/// <summary>
/// Фабрика для подписки наблюдателей на события юнитов
/// </summary>
public class ObserverUnitFactory : IUnitFactory
{
    private readonly IUnitFactory _innerFactory;

    public ObserverUnitFactory(IUnitFactory innerFactory)
    {
        _innerFactory = innerFactory;
    }

    public string UnitTypeName => _innerFactory.UnitTypeName;

    public IUnit CreateUnit()
    {
        var unit = _innerFactory.CreateUnit();
        
        // Подписываем наблюдателей на события юнита
        if (ProxySettings.Current.EnableDamageLogging)
        {
            unit.Subscribe(new DamageLoggerObserver());
        }

        if (ProxySettings.Current.EnableDeathBeep)
        {
            unit.Subscribe(new DeathBeepObserver());
        }

        return unit;
    }
}
