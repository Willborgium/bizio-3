using Hyjynx.Core.Services;
using Hyjynx.Bizio.Services;
using Hyjynx.Core;
using Hyjynx.Core.Scenes;

namespace Hyjynx.Bizio.Scenes
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
            IDataService dataService,
            InitializationArguments initializationArguments,
            Func<BizioScene> bizioSceneFactory
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
            _dataService = dataService;
            _bizioSceneFactory = bizioSceneFactory;
        }

        protected override void StartNewGame(object? sender, EventArgs e)
        {
            _dataService.InitializeNewGame(
                c =>
                {
                    //c.Allocations.CollectionChanged += OnCompanyAllocationsChanged;
                }
            );

            _loggingService
                .Info($"Game ID: {_dataService.CurrentGame.GameId}")
                .Info($"Person count: {_dataService.CurrentGame.People.Count}")
                .Info($"Company count: {_dataService.CurrentGame.Companies.Count}")
                .Info($"Project count: {_dataService.CurrentGame.Projects.Count}");

            _dataService.SendMessage(
                "HR",
                "Congrats on getting funded!",
                "Now that you've got some cash, why not hire an employee and try to complete a project?"
            );

            var bizioScene = _bizioSceneFactory();
            _sceneService.PushScene(bizioScene);
        }

        private readonly IDataService _dataService;
        private readonly Func<BizioScene> _bizioSceneFactory;
    }
}
