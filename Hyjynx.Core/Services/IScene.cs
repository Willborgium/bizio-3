using Hyjynx.Core.Rendering;

namespace Hyjynx.Core.Services
{
    public interface IScene : IUpdateable, IRenderable
    {
        void LoadContent();
        void RegisterEvents();
        void UnregisterEvents();
        void UnloadContent();
    }
}
