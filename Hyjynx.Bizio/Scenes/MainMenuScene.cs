using Hyjynx.Core.Rendering.Interface;
using Hyjynx.Core.Services;
using Hyjynx.Bizio.Services;
using System.Numerics;
using Hyjynx.Core.Debugging;
using Hyjynx.Core;

namespace Hyjynx.Bizio.Scenes
{
    public class MainMenuScene : Scene
    {
        private readonly IDataService _dataService;
        private readonly IUtilityService _utilityService;
        private readonly ISceneService _sceneService;
        private readonly IInputService _inputService;
        private readonly Func<BizioScene> _bizioSceneFactory;
        private readonly InitializationArguments _initializationArguments;
        private const int ScreenWidth = 1920;
        private const int ScreenHeight = 1080;

        public MainMenuScene(
            IResourceService resourceService,
            IContentService contentService,
            ILoggingService loggingService,
            IDataService dataService,
            IUtilityService utilityService,
            ISceneService sceneService,
            IInputService inputService,
            Func<BizioScene> bizioSceneFactory,
            InitializationArguments initializationArguments
            )
            : base(resourceService, contentService, loggingService)
        {
            _dataService = dataService;
            _utilityService = utilityService;
            _sceneService = sceneService;
            _inputService = inputService;
            _bizioSceneFactory = bizioSceneFactory;
            _initializationArguments = initializationArguments;
        }

        public override void LoadContent()
        {
            DebuggingService.IsDebuggingEnabled = _initializationArguments.IsDebugModeEnabled;

            if (_initializationArguments.IsDebugModeEnabled)
            {
                _visualRoot.AddChild(DebuggingService.CreateDebugContainer(_loggingService, _utilityService, _visualRoot, _initializationArguments));
            }

            var menuScrollContainer = new ScrollContainer(_inputService)
            {
                Dimensions = new Vector2(175, 222)
            };

            var menu = new StackContainer
            {
                Padding = Vector4.One * 10,
                Direction = LayoutDirection.Vertical,
                Identifier = "container-menu"
            };

            menuScrollContainer.AddChild(menu);

            menu.AddChild(_utilityService.CreateButton("New Game", 0, 0, 200, 50, StartNewGame));
            menu.AddChild(_utilityService.CreateButton("Quit", 0, 0, 200, 50, QuitGame));
            menu.AddChild(_utilityService.CreateButton("Quit 2", 0, 0, 200, 50, QuitGame));
            menu.AddChild(_utilityService.CreateButton("Quit 3", 0, 0, 200, 50, QuitGame));
            menu.AddChild(_utilityService.CreateButton("Quit 4", 0, 0, 200, 50, QuitGame));
            menu.AddChild(_utilityService.CreateButton("Quit 5", 0, 0, 200, 50, QuitGame));
            menu.AddChild(_utilityService.CreateButton("Quit 6", 0, 0, 200, 50, QuitGame));

            var x = (ScreenWidth - menuScrollContainer.Dimensions.X) / 2;
            var y = (ScreenHeight - menuScrollContainer.Dimensions.Y) / 2;

            menuScrollContainer.Position = new Vector2(x, y);

            _visualRoot.AddChild(menuScrollContainer);
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
