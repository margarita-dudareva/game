
using System;

public class Program
{
    static void Main()
    {
        // Инициализировать номер сессии для логирования
        DamageLogViewer.InitializeSessionNumber();
        
        // Загрузить сохраненные настройки прокси
        var settings = ProxySettings.Current;

        var gameFlow = new GameFlowController();

        while (true)
        {
            int choice = gameFlow.ShowMainMenu();

            switch (choice)
            {
                case 1:
                    gameFlow.StartNewGame();
                    break;
                case 2:
                    gameFlow.LoadSavedGame();
                    break;
                case 3:
                    gameFlow.ConfigureProxySettings();
                    break;
                case 4:
                    return;
            }
        }
    }
}