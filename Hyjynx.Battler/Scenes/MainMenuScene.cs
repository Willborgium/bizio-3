using Hyjynx.Battler.Services;
using Hyjynx.Core;
using Hyjynx.Core.Debugging;
using Hyjynx.Core.Rendering;
using Hyjynx.Core.Rendering.Interface;
using Hyjynx.Core.Services;
using Newtonsoft.Json;
using System.Numerics;

namespace Hyjynx.Battler.Scenes
{
    internal class MainMenuScene : Scene, IFirstScene
    {
        private enum MainMenuState
        {
            MainMenu,
            LoadGame,
            Options
        }

        private MainMenuState _state;

        public MainMenuScene(
            IResourceService resourceService,
            IContentService contentService,
            ILoggingService loggingService,
            IUtilityService utilityService,
            ISceneService sceneService,
            Func<GameWorldScene> gameWorldSceneFactory,
            Func<BattleScene> battleSceneFactory,
            InitializationArguments initializationArguments,
            IDevelopmentService developmentService)
            : base(resourceService, contentService, loggingService)
        {
            _utilityService = utilityService;
            _initializationArguments = initializationArguments;
            _sceneService = sceneService;
            _gameWorldSceneFactory = gameWorldSceneFactory;
            _battleSceneFactory = battleSceneFactory;
            _developmentService = developmentService;
            _state = MainMenuState.MainMenu;
        }

        private void CreateMainMenu(IFont font)
        {
            var menu = new StackContainer
            {
                Position = new Vector2(_initializationArguments.ScreenWidth / 2, _initializationArguments.ScreenHeight / 2),
                Padding = new Vector4(0, 10, 0, 10)
            };

            menu.Bind(m => m.IsVisible = _state == MainMenuState.MainMenu);

            menu.AddChild(_utilityService.CreateButton("New Game", OnNewGame));

            menu.AddChild(_utilityService.CreateButton("Load Game", OnLoadGame));

            if (DebuggingService.IsDebuggingEnabled)
            {
                menu.AddChild(_utilityService.CreateButton("Sample Battle", OnSampleBattle));
            }

            menu.AddChild(_utilityService.CreateButton("Options", OnOptions));

            menu.AddChild(_utilityService.CreateButton("Quit", OnQuit));

            _visualRoot.AddChild(menu);
        }

        private void CreateLoadGameMenu(IFont font)
        {
            var menu = new StackContainer
            {
                Position = new Vector2(_initializationArguments.ScreenWidth / 2, _initializationArguments.ScreenHeight / 2),
                Padding = new Vector4(0, 10, 0, 10)
            };

            menu.Bind(m => m.IsVisible = _state == MainMenuState.LoadGame);

            var saveDataFile = _resourceService.Get<SaveDataFile>(WellKnownResources.SaveDataFile);

            foreach (var game in saveDataFile.Games)
            {
                menu.AddChild(_utilityService.CreateButton($"{game.Id}", (s, e) => LoadGame(game)));
            }

            menu.AddChild(_utilityService.CreateButton("Back", OnBack));

            _visualRoot.AddChild(menu);
        }

        private void LoadGame(GameData game)
        {
        }

        private void CreateOptionsMenu(IFont font)
        {
            var menu = new StackContainer
            {
                Position = new Vector2(_initializationArguments.ScreenWidth / 2, _initializationArguments.ScreenHeight / 2),
                Padding = new Vector4(0, 10, 0, 10)
            };

            menu.Bind(m => m.IsVisible = _state == MainMenuState.Options);

            menu.AddChild(_utilityService.CreateButton("Back", OnBack));

            _visualRoot.AddChild(menu);
        }

        private void OnBack(object? sender, EventArgs e)
        {
            _state = MainMenuState.MainMenu;
        }

        public override void LoadContent()
        {
            var saveDataFile = LoadResource<SaveDataFile>(SaveDataFilePath, WellKnownResources.SaveDataFile);

            var font = _resourceService.Get<IFont>("font-default") ?? throw new Exception("Cannot find default font");

            CreateMainMenu(font);
            CreateLoadGameMenu(font);
            CreateOptionsMenu(font);
        }

        private void OnLoadGame(object? sender, EventArgs e)
        {
            _state = MainMenuState.LoadGame;
        }

        private void OnQuit(object? sender, EventArgs e)
        {
            _sceneService.PopScene();
        }

        private void OnOptions(object? sender, EventArgs e)
        {
            _state = MainMenuState.Options;
        }

        private void OnSampleBattle(object? sender, EventArgs e)
        {
            var battle = _developmentService.GenerateBattleData();

            _resourceService.Set("battle-data", battle);

            _sceneService.PushScene(_battleSceneFactory());
        }

        private void OnNewGame(object? sender, EventArgs e)
        {
        }

        private T LoadResource<T>(string path, string key)
        {
            string fileContents;

            using (var reader = new StreamReader(path))
            {
                fileContents = reader.ReadToEnd();
            }

            var data = JsonConvert.DeserializeObject<T>(fileContents);

            _resourceService.Set(key, data);

            return data;
        }

        public override void UnloadContent()
        {
        }

        private const string SaveDataFilePath = "saves.json";
        private readonly IUtilityService _utilityService;
        private readonly InitializationArguments _initializationArguments;
        private readonly ISceneService _sceneService;
        private readonly Func<GameWorldScene> _gameWorldSceneFactory;
        private readonly Func<BattleScene> _battleSceneFactory;
        private readonly IDevelopmentService _developmentService;
    }
}
