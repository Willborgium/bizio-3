using Hyjynx.Core.Rendering.Interface;
using Hyjynx.Core.Services;
using System.Numerics;

namespace Hyjynx.Core.Scenes
{
    public abstract class BaseMainMenuScene : Scene
    {
        protected readonly IInputService _inputService;
        protected readonly IUtilityService _utilityService;
        protected readonly ISceneService _sceneService;
        protected readonly InitializationArguments _initializationArguments;

        public BaseMainMenuScene(
            IResourceService resourceService,
            IContentService contentService,
            ILoggingService loggingService,
            IInputService inputService,
            IUtilityService utilityService,
            ISceneService sceneService,
            InitializationArguments initializationArguments
            )
            : base(
                  resourceService,
                  contentService,
                  loggingService
                  )
        {
            _inputService = inputService;
            _utilityService = utilityService;
            _sceneService = sceneService;
            _initializationArguments = initializationArguments;
        }

        public override void LoadContent()
        {
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

            var x = (_initializationArguments.ScreenWidth - menuScrollContainer.Dimensions.X) / 2;
            var y = (_initializationArguments.ScreenHeight - menuScrollContainer.Dimensions.Y) / 2;

            menuScrollContainer.Offset = new Vector2(x - menu.Dimensions.X / 2, y - menu.Dimensions.Y / 2);

            _visualRoot.AddChild(menuScrollContainer);
        }

        protected virtual void QuitGame(object? sender, EventArgs e) => _sceneService.PopScene();

        protected abstract void StartNewGame(object? sender, EventArgs e);
    }
}
