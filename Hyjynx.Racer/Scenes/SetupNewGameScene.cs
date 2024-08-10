using Hyjynx.Core.Services;
using Hyjynx.Racer.Models;

namespace Hyjynx.Racer.Scenes
{
    public class SetupNewGameScene : Scene
    {
        public SetupNewGameScene(
            IResourceService resourceService,
            IContentService contentService,
            ILoggingService loggingService,
            IUtilityService utilityService,
            ISceneService sceneService,
            Func<TestGameplayScene> testGameplaySceneFactory
            )
            : base(
                  resourceService,
                  contentService,
                  loggingService
                  )
        {
            _utilityService = utilityService;
            _sceneService = sceneService;
            _testGameplaySceneFactory = testGameplaySceneFactory;


            _newGameData = new()
            {
                Id = Guid.NewGuid()
            };
        }

        public override void LoadContent()
        {
            var startButton = _utilityService.CreateButton("Start Game", OnStartGame);

            _visualRoot.AddChild(startButton);
        }

        private void OnStartGame(object? sender, EventArgs e)
        {
            var gameData = new GameData
            {
                Id = _newGameData.Id
            };

            _resourceService.Set("current-game", gameData);

            _sceneService.SwapScene(_testGameplaySceneFactory());
        }

        private readonly NewGameData _newGameData;

        private readonly IUtilityService _utilityService;
        private readonly ISceneService _sceneService;
        private readonly Func<TestGameplayScene> _testGameplaySceneFactory;
    }
}
