using Hyjynx.Battler.Model;
using Hyjynx.Core;
using Hyjynx.Core.Debugging;
using Hyjynx.Core.Rendering;
using Hyjynx.Core.Rendering.Interface;
using Hyjynx.Core.Services;
using Newtonsoft.Json;
using System.Drawing;
using System.Numerics;

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
            InitializationArguments initializationArguments)
            : base(resourceService, contentService, loggingService)
        {
            _utilityService = utilityService;
            _initializationArguments = initializationArguments;
            _sceneService = sceneService;
            _gameWorldSceneFactory = gameWorldSceneFactory;
            _battleSceneFactory = battleSceneFactory;
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
            SetupSampleBattle();
        }

        private void OnNewGame(object? sender, EventArgs e)
        {
        }

        private readonly IEnumerable<BattlerAttackData> _attacks = new[]
        {
            new BattlerAttackData
            {
                Name = "Kick",
                Power = 5,
                MaxPowerPoints = 20,
                PowerPoints = 20
            },
            new BattlerAttackData
            {
                Name = "Drop Kick",
                Power = 8,
                MaxPowerPoints = 10,
                PowerPoints = 10
            },
            new BattlerAttackData
            {
                Name = "Punch",
                Power = 10,
                MaxPowerPoints = 10,
                PowerPoints = 10
            },
            new BattlerAttackData
            {
                Name = "Slash",
                Power = 15,
                MaxPowerPoints = 5,
                PowerPoints = 5
            }
        };

        private readonly IEnumerable<BattlerToolData> _tools = new[]
        {
            new BattlerToolData
            {
                Name = "X Attack",
                Count = 3
            },
            new BattlerToolData
            {
                Name = "X Defense",
                Count = 3
            },
            new BattlerToolData
            {
                Name = "X Speed",
                Count = 3
            },
            new BattlerToolData
            {
                Name = "X HP",
                Count = 3
            }
        };

        private ICollection<BattlerAttackData> GenerateRandomAttacks(int count, byte? powerPoints = null)
        {
            var output = new List<BattlerAttackData>();

            for (var index = 0; index < count; index++)
            {
                var attack = _attacks.Random(a => !output.Any(o => o.Name == a.Name));

                output.Add(new BattlerAttackData
                {
                    Name = attack.Name,
                    Power = attack.Power,
                    MaxPowerPoints = attack.MaxPowerPoints,
                    PowerPoints = powerPoints ?? attack.PowerPoints
                });
            }

            return output;
        }

        private ICollection<BattlerToolData> GenerateRandomTools(int distinctToolCount, byte? countPerTool = null)
        {
            var output = new List<BattlerToolData>();

            for (var index = 0; index < distinctToolCount; index++)
            {
                var tool = _tools.Random(a => !output.Any(o => o.Name == a.Name));

                output.Add(new BattlerToolData
                {
                    Name = tool.Name,
                    Count = countPerTool ?? tool.Count
                });
            }

            return output;
        }

        private void SetupSampleBattle()
        {
            var b1 = new BattlerData
            {
                Id = Guid.NewGuid(),
                Name = "Player Battler",
                Health = (short)Random.Shared.Next(50, 100),
                Attacks = GenerateRandomAttacks(2),
                Tools = GenerateRandomTools(2)
            };

            var b2 = new BattlerData
            {
                Id = Guid.NewGuid(),
                Name = "AI Battler",
                Health = (short)Random.Shared.Next(50, 100),
                Attacks = GenerateRandomAttacks(2, 2),
                Tools = GenerateRandomTools(2)
            };

            var battle = new BattleData
            {
                PlayerBattler = b1,
                ComputerBattler = b2,
            };

            _resourceService.Set("battle-data", battle);

            _sceneService.PushScene(_battleSceneFactory());
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
