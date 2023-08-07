using Hyjynx.Core.Rendering;

namespace Hyjynx.Core.Services
{
    public interface IScene
    {
        void Update();
        void Render(IRenderer renderer);
        void LoadContent();
        void RegisterEvents();
        void UnregisterEvents();
        void UnloadContent();
    }
}
