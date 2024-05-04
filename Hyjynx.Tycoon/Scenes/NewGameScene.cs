using Hyjynx.Core.Services;
using Hyjynx.Tycoon.Models;

namespace Hyjynx.Tycoon.Scenes
{
    internal class NewGameScene : Scene
    {
        public NewGameScene(
            IResourceService resourceService,
            IContentService contentService,
            ILoggingService loggingService,
            ISceneService sceneService,
            Func<GameScene> gameSceneFactory
            )
            : base(
                  resourceService,
                  contentService,
                  loggingService
                  )
        {
            _sceneService = sceneService;
            _gameSceneFactory = gameSceneFactory;
        }

        public override void LoadContent()
        {
            _loggingService.Info("New game created");

            var gameData = new GameData
            {
                Id = Guid.NewGuid(),
                Created = DateTime.Now,
                Updated = DateTime.Now
            };

            _resourceService.Set(KnownResources.GameData, gameData);

            _sceneService.SwapScene(_gameSceneFactory());
        }

        private readonly ISceneService _sceneService;
        private readonly Func<GameScene> _gameSceneFactory;
    }
}
