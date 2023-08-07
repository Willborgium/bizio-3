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
        private readonly Func<MainMenuScene> _mainMenuSceneFactory;
        private readonly InitializationArguments _initializationArguments;

        public InitializationScene(
            IResourceService resourceService,
            IContentService contentService,
            ILoggingService loggingService,
            IDataService dataService,
            ISceneService sceneService,
            Func<MainMenuScene> mainMenuSceneFactory,
            InitializationArguments initializationArguments
            )
            : base(resourceService, contentService, loggingService)
        {
            _dataService = dataService;
            _sceneService = sceneService;
            _mainMenuSceneFactory = mainMenuSceneFactory;
            _initializationArguments = initializationArguments;
        }

        public override void LoadContent()
        {
            DebuggingService.IsDebuggingEnabled = _initializationArguments.IsDebugModeEnabled;

            var font = _contentService.Load<IFont>("font-default");
            _resourceService.Set("font-default", font);
            DebuggingService.Font = font;

            var pixel = _contentService.Load<ITexture2D>("pixel");
            _resourceService.Set("texture-pixel", pixel);
            DebuggingService.PixelTexture = pixel;

            _loggingService.Initialize(font, pixel);

            var buttonSpritesheet = _contentService.Load<ITexture2D>("greySheet");
            var buttonMetadata = new ButtonMetadata
            {
                Font = font,
                Spritesheet = buttonSpritesheet,
                DefaultSource = new Rectangle(0, 143, 190, 45),
                HoveredSource = new Rectangle(0, 98, 190, 45),
                ClickedSource = new Rectangle(0, 188, 190, 49),
                DisabledSource = new Rectangle(0, 0, 195, 49)
            };

            _resourceService.Set("button-spritesheet-default", buttonSpritesheet);
            _resourceService.Set("button-metadata-default", buttonMetadata);

            _dataService.Initialize();

            _loggingService.Info($"Skill count: {_dataService.StaticData.Skills.Count}");

            // Initialization should only happen once, so swap instead of push
            _sceneService.SwapScene(_mainMenuSceneFactory());
        }
    }
}
