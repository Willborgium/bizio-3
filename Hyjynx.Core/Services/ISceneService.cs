using Hyjynx.Core.Rendering;
using System;

namespace Hyjynx.Core.Services
{
    public interface ISceneService : IRenderable
    {
        event EventHandler<SceneChangedEventArgs> SceneChanged;

        void PushScene(IScene scene);
        void PopScene();
        void SwapScene(IScene scene);
        bool Update();
    }
}