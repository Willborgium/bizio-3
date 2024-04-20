using Hyjynx.Core.Services;

namespace Hyjynx.Core.Scenes
{
    public class DefaultInitializationScene<TFirstScene> : Scene, IFirstScene
        where TFirstScene : Scene
    {
        public DefaultInitializationScene(
            IResourceService resourceService,
            IContentService contentService,
            ILoggingService loggingService,
            IUtilityService utilityService,
            ISceneService sceneService,
            Func<TFirstScene> firstSceneFactory
            )
            : base(resourceService, contentService, loggingService)
        {
            _utilityService = utilityService;
            _sceneService = sceneService;
            _firstSceneFactory = firstSceneFactory;
        }

        public override void LoadContent()
        {
            _utilityService.InitializeLogging(_contentService);
            _utilityService.TryAddDebuggingContainer(_visualRoot);

            _sceneService.SwapScene(_firstSceneFactory());
        }

        private readonly IUtilityService _utilityService;
        private readonly ISceneService _sceneService;
        private readonly Func<TFirstScene> _firstSceneFactory;
    }
}
