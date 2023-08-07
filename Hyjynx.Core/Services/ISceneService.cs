using System;

namespace Hyjynx.Core.Services
{
    public interface ISceneService
    {
        event EventHandler<SceneChangedEventArgs> SceneChanged;

        void PushScene(IScene scene);
        void PopScene();
        void SwapScene(IScene scene);
        void Render();
        bool Update();
    }
}