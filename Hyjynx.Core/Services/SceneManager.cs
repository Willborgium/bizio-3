using Hyjynx.Core.Rendering;
using System.Drawing;

namespace Hyjynx.Core.Services
{
    public interface IFirstScene : IScene { }

    public class SceneService : ISceneService
    {
        public event EventHandler<SceneChangedEventArgs>? SceneChanged;

        public SceneService(IRenderer renderer, Func<IFirstScene> firstSceneProvider)
        {
            _renderer = renderer;
            _firstSceneProvider = firstSceneProvider;
            _scenes = new Stack<SceneContainer>();
            _isLoaded = false;
        }

        public void PushScene(IScene scene) => _nextScene = scene;

        public void PopScene() => _popSceneCount++;

        public void SwapScene(IScene scene)
        {
            PopScene();
            PushScene(scene);
        }

        public bool Update()
        {
            TryPushFirstScene();

            SceneContainer? currentScene = null;

            if (_scenes.Any())
            {
                currentScene = _scenes.Peek();

                if (currentScene.State == SceneState.Unloaded)
                {
                    currentScene.Scene.LoadContent();
                    currentScene.State = SceneState.Loaded;
                }

                if (currentScene.State == SceneState.Loaded)
                {
                    currentScene.Scene.RegisterEvents();
                    currentScene.State = SceneState.Registered;
                }

                currentScene.Scene.Update();
                currentScene.State = SceneState.Running;
            }

            var didPopScene = _popSceneCount > 0;

            while (_popSceneCount > 0)
            {
                var scene = _scenes.Pop();

                if (scene.State == SceneState.Running ||
                    scene.State == SceneState.Registered)
                {
                    scene.Scene.UnregisterEvents();
                    scene.State = SceneState.Loaded;
                }

                if (scene.State == SceneState.Loaded)
                {
                    scene.Scene.UnloadContent();
                    scene.State = SceneState.Unloaded;
                }

                _popSceneCount--;
            }

            if (_nextScene == null)
            {
                return _scenes.Any();
            }

            SceneChanged?.Invoke(this, new SceneChangedEventArgs(currentScene?.Scene, _nextScene));

            if (!didPopScene && currentScene != null)
            {
                currentScene.Scene.UnregisterEvents();
                currentScene.State = SceneState.Loaded;
            }

            _scenes.Push(new SceneContainer
            {
                State = SceneState.Unloaded,
                Scene = _nextScene
            });

            _nextScene = null;

            return _scenes.Any();
        }

        public void Render()
        {
            _scenes.Peek().Scene.Render(_renderer);
        }

        private void TryPushFirstScene()
        {
            if (_isLoaded)
            {
                return;
            }

            var firstScene = _firstSceneProvider?.Invoke() ?? throw new InvalidOperationException("First scene is null");

            PushScene(firstScene);

            _isLoaded = true;
        }

        private IScene? _nextScene;
        private int _popSceneCount;

        private readonly Stack<SceneContainer> _scenes;
        private readonly IRenderer _renderer;
        private readonly Func<IFirstScene> _firstSceneProvider;
        private bool _isLoaded;

        private class SceneContainer
        {
            public SceneState State { get; set; }
            public IScene Scene { get; set; }
        }

        private enum SceneState
        {
            Unloaded,
            Loaded,
            Registered,
            Running
        }
    }
}
