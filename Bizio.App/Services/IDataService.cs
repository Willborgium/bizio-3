using Bizio.App.Game;
using System;

namespace Bizio.App.Services
{
    public interface IDataService
    {
        GameData CurrentGame { get; }
        StaticData StaticData { get; }

        void Initialize();
        void InitializeNewGame(Action<GameData> globalInitializeHook, Action<Company> playerCompanyInitializeHook);
    }
}