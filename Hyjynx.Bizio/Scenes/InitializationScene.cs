using Hyjynx.Core.Services;
using Hyjynx.Bizio.Services;
using Hyjynx.Core.Scenes;

namespace Hyjynx.Bizio.Scenes
{
    public class InitializationScene : DefaultInitializationScene<MainMenuScene>
    {
        public InitializationScene(
            IResourceService resourceService,
            IContentService contentService,
            ILoggingService loggingService,
            IUtilityService utilityService,
            ISceneService sceneService,
            IDataService dataService,
            Func<MainMenuScene> firstSceneFactory
            )
            : base(
                  resourceService,
                  contentService,
                  loggingService,
                  utilityService,
                  sceneService,
                  firstSceneFactory
                  )
        {
            _dataService = dataService;
        }

        public override void LoadContent()
        {
            _dataService.Initialize();

            _loggingService.Info($"Skill count: {_dataService.StaticData.Skills.Count}");

            base.LoadContent();
        }

        private readonly IDataService _dataService;

    }
}
