using Hyjynx.Core;
using Hyjynx.Core.Scenes;
using Hyjynx.Core.Services;
using Hyjynx.Tycoon.Models;

namespace Hyjynx.Tycoon.Scenes
{
    internal class MainMenuScene : BaseMainMenuScene
    {
        public MainMenuScene(
            IResourceService resourceService,
            IContentService contentService,
            ILoggingService loggingService,
            IInputService inputService,
            IUtilityService utilityService,
            ISceneService sceneService,
            Func<NewGameScene> newGameSceneFactory,
            InitializationArguments initializationArguments
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
            _newGameSceneFactory = newGameSceneFactory;
        }

        public override void LoadContent()
        {
            _loggingService.Info("Main menu loaded");

            base.LoadContent();
        }

        protected override void StartNewGame(object? sender, EventArgs e)
        {
            _sceneService.PushScene(_newGameSceneFactory());
        }

        private readonly Func<NewGameScene> _newGameSceneFactory;
    }
}
