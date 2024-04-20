using Hyjynx.Core;
using Hyjynx.Core.Scenes;
using Hyjynx.Core.Services;

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
        }

        protected override void StartNewGame(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
