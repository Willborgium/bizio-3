using Hyjynx.Battler.Model;
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
            Func<BattleScene> battleSceneFactory
            )
            : base(resourceService, contentService, loggingService)
        {
            _utilityService = utilityService;
            _sceneService = sceneService;
            _battleSceneFactory = battleSceneFactory;
        }

        public override void LoadContent()
        {
            _utilityService.InitializeLogging(_contentService);
            _utilityService.TryAddDebuggingContainer(_visualRoot);

            // Sample battle setup

            var b1 = new BattlerData
            {
                Id = Guid.NewGuid(),
                Name = "Player Battler",
                Health = 100,
                Attacks = new List<BattlerAttackData>
                {
                    new BattlerAttackData
                    {
                        Name = "Punch",
                        Power = 10
                    },
                    new BattlerAttackData
                    {
                        Name = "Kick",
                        Power = 5
                    }
                }
            };

            var b2 = new BattlerData
            {
                Id = Guid.NewGuid(),
                Name = "AI Battler",
                Health = 100,
                Attacks = new List<BattlerAttackData>
                {
                    new BattlerAttackData
                    {
                        Name = "Punch",
                        Power = 10
                    },
                    new BattlerAttackData
                    {
                        Name = "Kick",
                        Power = 5
                    }
                }
            };

            var battle = new BattleData
            {
                PlayerBattler = b1,
                ComputerBattler = b2,
            };

            _resourceService.Set("battle-data", battle);


            _sceneService.SwapScene(_battleSceneFactory());
        }

        private readonly IUtilityService _utilityService;
        private readonly ISceneService _sceneService;
        private readonly Func<BattleScene> _battleSceneFactory;
    }
}
