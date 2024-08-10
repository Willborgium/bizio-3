using Hyjynx.Core.Services;
using Hyjynx.Core;
using Hyjynx.Core.Scenes;

namespace Hyjynx.Racer.Scenes
{
    public class MainMenuScene : BaseMainMenuScene
    {
        public MainMenuScene(
            IResourceService resourceService,
            IContentService contentService,
            ILoggingService loggingService,
            IInputService inputService,
            IUtilityService utilityService,
            ISceneService sceneService,
            InitializationArguments initializationArguments,
            Func<SetupNewGameScene> setupNewGameSceneFactory
            )
            : base(
                  resourceService,
                  contentService,
                  loggingService,
                  inputService,
                  utilityService,
                  sceneService,
                  initializationArguments
                  )
        {
            _setupNewGameSceneFactory = setupNewGameSceneFactory;
        }

        protected override void StartNewGame(object? sender, EventArgs e)
        {
            _sceneService.PushScene(_setupNewGameSceneFactory());
        }

        private readonly Func<SetupNewGameScene> _setupNewGameSceneFactory;
    }
}
