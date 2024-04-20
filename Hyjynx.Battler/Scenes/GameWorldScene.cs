using Hyjynx.Core.Services;

namespace Hyjynx.Battler.Scenes
{
    internal class GameWorldScene : Scene, IFirstScene
    {
        public GameWorldScene(
            IResourceService resourceService,
            IContentService contentService,
            ILoggingService loggingService,
            IUtilityService utilityService,
            ISceneService sceneService,
            Func<BattleScene> battleSceneFactory)
            : base(resourceService, contentService, loggingService)
        {
        }
    }

    public static class WellKnownResources
    {
        public const string SaveDataFile = "save-data-file";
    }

    internal class SaveDataFile
    {
        public ICollection<GameData> Games { get; set; }
    }

    internal class GameData
    {
        public Guid Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}
