using Hyjynx.Bizio.Game;

namespace Hyjynx.Bizio.Services
{
    public interface IDataService
    {
        GameData CurrentGame { get; }
        StaticData StaticData { get; }

        void Initialize();
        void InitializeNewGame(Action<Company> playerCompanyInitializeHook);
        void ProcessTurn();
        void SendMessage(string from, string subject, string body);
    }
}