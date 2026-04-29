using System;
using System.Reflection;

namespace Game.Models.Units.Adapters
{
    /// <summary>
    /// Адаптер для преобразования Гуляй-города в IUnit
    /// Использует Adapter Pattern для совместимости с боевой системой
    /// Гуляй-город - это мощная средневековая крепость с высокой защитой
    /// Пытается загрузить реальный Гуляй-город из MedievalRussia.dll через рефлексию
    /// </summary>
    public class GulyayGorodAdapter : UnitBase
    {
        /// <summary>
        /// Создать адаптер для Гуляй-города с заданным уровнем
        /// Гуляй-город - это мощная крепость без атаки, с очень высокой защитой и большим запасом здоровья
        /// </summary>
        public GulyayGorodAdapter() 
            : base(
                name: "Gulyay-Gorod",
                attack: 0, // Крепость не атакует
                defense: 25, // Очень высокая защита, растёт с уровнем
                health: 55, // Большой запас здоровья, растёт с уровнем
                cost: 80  // Дорогой юнит
            )
        {
            // Пытаемся загрузить реальный Гуляй-город из MedievalRussia.dll через рефлексию
            // для возможного использования его исторических характеристик в будущем
            ValidateExternalLibrary();
        }

        /// <summary>
        /// Создать адаптер для Гуляй-города с заданными параметрами (для загрузки из сохранения)
        /// </summary>
        public GulyayGorodAdapter(int attack, int defense, int health, int cost)
            : base(
                name: "Gulyay-Gorod",
                attack: attack,
                defense: defense,
                health: health,
                cost: cost
            )
        {
        }

        /// <summary>
        /// Проверить, доступна ли внешняя библиотека MedievalRussia.dll
        /// </summary>
        private static void ValidateExternalLibrary()
        {
            try
            {
                var gulyayGorodType = Type.GetType("MedievalRussia.GulyayGorod, MedievalRussia");
                if (gulyayGorodType == null)
                {
                    // DLL не загружена - это нормально, работаем с локальными характеристиками
                    return;
                }

                // Если удалось загрузить тип, пытаемся создать экземпляр
                TryCreateInstance(gulyayGorodType);
            }
            catch
            {
                // Если DLL не доступна или возникла ошибка, просто продолжаем работу
                // с локальными характеристиками адаптера
            }
        }

        /// <summary>
        /// Попытка создать экземпляр GulyayGorod через различные конструкторы
        /// </summary>
        private static object? TryCreateInstance(Type gulyayGorodType)
        {
            try
            {
                // Сначала пробуем конструктор с 2 параметрами int
                var constructor = gulyayGorodType.GetConstructor(new[] { typeof(int), typeof(int) });
                if (constructor != null)
                    return Activator.CreateInstance(gulyayGorodType, 1, 0);

                // Пробуем конструктор с 1 параметром int
                constructor = gulyayGorodType.GetConstructor(new[] { typeof(int) });
                if (constructor != null)
                    return Activator.CreateInstance(gulyayGorodType, 1);

                // Пробуем конструктор без параметров
                constructor = gulyayGorodType.GetConstructor(Type.EmptyTypes);
                if (constructor != null)
                    return Activator.CreateInstance(gulyayGorodType);
            }
            catch { }

            return null;
        }
    }
}