using Hyjynx.Battler.Model;
using Hyjynx.Core;
using Hyjynx.Core.Services;

namespace Hyjynx.Battler.Scenes
{
    internal class InitializationScene : Scene, IFirstScene
    {
        public InitializationScene(
            IResourceService resourceService,
            IContentService contentService,
            ILoggingService loggingService,
            IUtilityService utilityService,
            ISceneService sceneService,
            Func<MainMenuScene> mainMenuSceneFactory
            )
            : base(resourceService, contentService, loggingService)
        {
            _utilityService = utilityService;
            _sceneService = sceneService;
            _mainMenuSceneFactory = mainMenuSceneFactory;
        }

        public override void LoadContent()
        {
            _utilityService.InitializeLogging(_contentService);
            _utilityService.TryAddDebuggingContainer(_visualRoot);

            _sceneService.SwapScene(_mainMenuSceneFactory());
        }

        private readonly IUtilityService _utilityService;
        private readonly ISceneService _sceneService;
        private readonly Func<MainMenuScene> _mainMenuSceneFactory;
    }
}
