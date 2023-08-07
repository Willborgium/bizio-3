using Hyjynx.Core.Rendering.Interface;
using Hyjynx.Core.Services;
using Hyjynx.Bizio.Services;
using System.Numerics;

namespace Hyjynx.Bizio.Scenes
{
    public class MainMenuScene : Scene
    {
        private readonly IDataService _dataService;
        private readonly IUtilityService _utilityService;
        private readonly ISceneService _sceneService;
        private readonly Func<BizioScene> _bizioSceneFactory;

        private const int ScreenWidth = 1920;
        private const int ScreenHeight = 1080;

        public MainMenuScene(
            IResourceService resourceService,
            IContentService contentService,
            ILoggingService loggingService,
            IDataService dataService,
            IUtilityService utilityService,
            ISceneService sceneService,
            Func<BizioScene> bizioSceneFactory
            )
            : base(resourceService, contentService, loggingService)
        {
            _dataService = dataService;
            _utilityService = utilityService;
            _sceneService = sceneService;
            _bizioSceneFactory = bizioSceneFactory;
        }

        public override void LoadContent()
        {
            var menu = new StackContainer
            {
                Padding = Vector4.One * 10,
                Direction = LayoutDirection.Vertical,
                Identifier = "container-menu"
            };

            menu.AddChild(_utilityService.CreateButton("New Game", 0, 0, 200, 50, StartNewGame));
            menu.AddChild(_utilityService.CreateButton("Quit", 0, 0, 200, 50, QuitGame));

            var x = (ScreenWidth - menu.Dimensions.X) / 2;
            var y = (ScreenHeight - menu.Dimensions.Y) / 2;

            menu.Position = new Vector2(x, y);

            _visualRoot.AddChild(menu);
        }

        private void QuitGame(object sender, EventArgs e) => _sceneService.PopScene();

        private void StartNewGame(object sender, EventArgs e)
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
    }
}
