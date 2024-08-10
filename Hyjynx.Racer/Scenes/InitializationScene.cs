using Hyjynx.Core.Services;
using Hyjynx.Core.Scenes;

namespace Hyjynx.Racer.Scenes
{
    public class InitializationScene : DefaultInitializationScene<MainMenuScene>
    {
        public InitializationScene(
            IResourceService resourceService,
            IContentService contentService,
            ILoggingService loggingService,
            IUtilityService utilityService,
            ISceneService sceneService,
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
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }
    }
}
