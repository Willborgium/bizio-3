using Hyjynx.Core.Debugging;
using Hyjynx.Core.Rendering.Interface;
using Hyjynx.Core.Rendering;
using Hyjynx.Core.Services;
using System.Drawing;
using Hyjynx.Bizio.Services;
using Hyjynx.Core;

namespace Hyjynx.Bizio.Scenes
{
    public class InitializationScene : Scene, IFirstScene
    {
        private readonly IDataService _dataService;
        private readonly ISceneService _sceneService;
        private readonly IUtilityService _utilityService;
        private readonly Func<MainMenuScene> _mainMenuSceneFactory;

        public InitializationScene(
            IResourceService resourceService,
            IContentService contentService,
            ILoggingService loggingService,
            IDataService dataService,
            ISceneService sceneService,
            IUtilityService utilityService,
            Func<MainMenuScene> mainMenuSceneFactory
            )
            : base(resourceService, contentService, loggingService)
        {
            _dataService = dataService;
            _sceneService = sceneService;
            _utilityService = utilityService;
            _mainMenuSceneFactory = mainMenuSceneFactory;
        }

        public override void LoadContent()
        {
            _utilityService.InitializeLogging(_contentService);

            _dataService.Initialize();

            _loggingService.Info($"Skill count: {_dataService.StaticData.Skills.Count}");

            // Initialization should only happen once, so swap instead of push
            _sceneService.SwapScene(_mainMenuSceneFactory());
        }
    }
}
