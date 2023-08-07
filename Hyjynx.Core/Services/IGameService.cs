namespace Hyjynx.Core.Services
{
    public interface IGameService
    {
        event EventHandler OnExit;
        void Initialize();
        void LoadContent();
        void Update();
        void Render();
    }
}
