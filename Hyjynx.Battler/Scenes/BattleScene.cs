using Hyjynx.Battler.Model;
using Hyjynx.Core;
using Hyjynx.Core.Rendering;
using Hyjynx.Core.Rendering.Interface;
using Hyjynx.Core.Services;
using System.Drawing;
using System.Numerics;

namespace Hyjynx.Battler.Scenes
{
    internal class BattleScene : Scene
    {
        private BattleState _battleState;

        public BattleScene(
            IResourceService resourceService,
            IContentService contentService,
            ILoggingService loggingService,
            IUtilityService utilityService,
            InitializationArguments initializationArguments
            )
            : base(resourceService, contentService, loggingService)
        {
            _utilityService = utilityService;
            _initializationArguments = initializationArguments;
            _battleState = BattleState.PlayerMainMenu;
        }

        public override void LoadContent()
        {
            _utilityService.TryAddDebuggingContainer(_visualRoot);

            var battleData = _resourceService.Get<BattleData>("battle-data") ?? throw new Exception("Missing battle data");

            var font = _resourceService.Get<IFont>("font-default") ?? throw new Exception("Cannot find default font");

            var state = new TextBox
            {
                Color = Color.Black,
                Font = font,
                Position = new Vector2(_initializationArguments.ScreenWidth / 2, _initializationArguments.ScreenHeight / 2)
            };

            state.Bind(tb =>
            {
                switch (_battleState)
                {
                    case BattleState.PlayerExecutingAction:
                        if (_playerBattleAction == null)
                        {
                            SetBattleState(BattleState.Exception);
                            break;
                        }
                        tb.Text = GenerateBattleActionDescription("Player", _playerBattleAction);
                        break;

                    case BattleState.ComputerExecutingAction:
                        if (_computerBattleAction == null)
                        {
                            SetBattleState(BattleState.Exception);
                            break;
                        }
                        tb.Text = GenerateBattleActionDescription("Computer", _computerBattleAction);
                        break;

                    default:
                        tb.Text = $"{_battleState}";
                        break;
                }
            });

            _visualRoot.AddChild(state);

            var playerContainer = new VisualContainer
            {
                CreateBattleStatsContainer(battleData.PlayerBattler),
                CreatePlayerMainMenu(battleData.PlayerBattler),
                CreatePlayerAttackMenu(battleData.PlayerBattler),
                CreatePlayerToolsMenu(battleData.PlayerBattler)
            };

            playerContainer.Position = new Vector2(0, _initializationArguments.ScreenHeight - playerContainer.Dimensions.Y);

            _visualRoot.AddChild(playerContainer);

            var aiStatsContainer = CreateBattleStatsContainer(battleData.ComputerBattler);

            _visualRoot.AddChild(aiStatsContainer);
        }

        private static string GenerateBattleActionDescription(string label, BattleAction action)
        {
            var output = $"{label} battler '{action.Battler.Name}' ";

            if (action.Attack != null)
            {
                output += $"executing attack '{action.Attack.Name}'";
            }
            else if (action.Tool != null)
            {
                output += $"using tool '{action.Tool.Name}'";
            }
            else
            {
                output += "no-op: unknown action";
            }

            return output;
        }

        private ContainerBase CreateBattleStatsContainer(BattlerData battler)
        {
            var font = _resourceService.Get<IFont>("font-default") ?? throw new Exception("Cannot find default font");
            
            var container = new StackContainer
            {
                Position = Vector2.Zero,
                Direction = LayoutDirection.Vertical,
                Padding = new Vector4(0, 10, 0, 10)
            };

            var playerName = new TextBox
            {
                Font = font,
                Color = Color.Black,
                Text = battler.Name
            };

            container.AddChild(playerName);

            var playerHealth = new TextBox
            {
                Font = font,
                Color = Color.Black,
                Text = "1"
            };

            playerHealth.Bind(tb => tb.Text = $"{battler.Health}");

            container.AddChild(playerHealth);

            return container;
        }

        private ContainerBase CreatePlayerMainMenu(BattlerData battler)
        {
            var container = new StackContainer
            {
                Direction = LayoutDirection.Horizontal,
                Padding = new Vector4(15, 0, 15, 0),
                Position = new Vector2(300, 0)
            };

            var attackButton = _utilityService.CreateButton("Attack", OnAttackButtonPressed);
            attackButton.Bind(b => b.IsEnabled = battler.Attacks.Sum(a => a.PowerPoints) > 0);
            container.AddChild(attackButton);

            var toolsButton = _utilityService.CreateButton("Tools", OnToolsButtonPressed);
            toolsButton.Bind(b => b.IsEnabled = battler.Tools.Sum(t => t.Count) > 0);
            container.AddChild(toolsButton);

            var retreatButton = _utilityService.CreateButton("Retreat", OnRetreatButtonPressed);
            retreatButton.Bind(b => b.IsEnabled = CanTryRetreat());
            container.AddChild(retreatButton);

            container.Bind(c => c.IsVisible = _battleState == BattleState.PlayerMainMenu);

            return container;
        }

        private ContainerBase CreatePlayerAttackMenu(BattlerData battler)
        {
            var container = new StackContainer
            {
                Direction = LayoutDirection.Horizontal,
                Padding = new Vector4(15, 0, 15, 0),
                Position = new Vector2(300, 0)
            };

            foreach (var attack in  battler.Attacks)
            {
                var button = _utilityService.CreateButton(attack.Name, (s, e) => OnPlayerAttackSelected(battler, attack));

                button.Bind(b => b.Text = $"{attack.Name} ({attack.PowerPoints}/{attack.MaxPowerPoints})");

                button.Bind(b => b.IsEnabled = attack.PowerPoints > 0);

                container.AddChild(button);
            }

            container.AddChild(_utilityService.CreateButton("Back", (s, e) => SetBattleState(BattleState.PlayerMainMenu)));

            container.Bind(c => c.IsVisible = _battleState == BattleState.PlayerAttackMenu);

            return container;
        }

        private ContainerBase CreatePlayerToolsMenu(BattlerData battler)
        {
            var container = new StackContainer
            {
                Direction = LayoutDirection.Horizontal,
                Padding = new Vector4(15, 0, 15, 0),
                Position = new Vector2(300, 0)
            };

            foreach (var tool in battler.Tools)
            {
                var button = _utilityService.CreateButton(tool.Name, (s, e) => OnPlayerToolSelected(battler, tool));

                button.Bind(b => b.Text = $"{tool.Name} ({tool.Count})");

                button.Bind(b =>
                {
                    if (tool.Count <= 0)
                    {
                        container.RemoveChild(b);
                    }
                });

                container.AddChild(button);
            }

            container.AddChild(_utilityService.CreateButton("Back", (s, e) => SetBattleState(BattleState.PlayerMainMenu)));

            container.Bind(c => c.IsVisible = _battleState == BattleState.PlayerToolsMenu);

            return container;
        }

        private void OnPlayerAttackSelected(BattlerData battler, BattlerAttackData battlerAttack)
        {
            _playerBattleAction = new BattleAction(battler, battlerAttack, null);

            SetBattleState(BattleState.PlayerAttackSelected);
        }

        private void OnPlayerToolSelected(BattlerData battler, BattlerToolData battlerTool)
        {
            _playerBattleAction = new BattleAction(battler, null, battlerTool);

            SetBattleState(BattleState.PlayerToolSelected);
        }

        private void SetBattleState(BattleState battleState)
        {
            _battleState = battleState;

            BattleData battleData;

            switch (_battleState)
            {
                case BattleState.PlayerAttackSelected:
                case BattleState.PlayerToolSelected:
                    GenerateComputerBattleAction();
                    break;

                case BattleState.ComputerAttackSelected:
                // case BattleState.ComputerToolSelected:
                    SetBattleState(BattleState.PlayerExecutingAction);
                    break;

                case BattleState.PlayerExecutingAction:
                    battleData = _resourceService.Get<BattleData>("battle-data") ?? throw new Exception("Missing battle data");

                    ProcessTurn(
                        _playerBattleAction ?? throw new Exception("No player battle action set"),
                        battleData.PlayerBattler,
                        battleData.ComputerBattler,
                        BattleState.PlayerWins,
                        BattleState.ComputerWins,
                        BattleState.ComputerExecutingAction
                    );
                    break;

                case BattleState.ComputerExecutingAction:
                    battleData = _resourceService.Get<BattleData>("battle-data") ?? throw new Exception("Missing battle data");

                    ProcessTurn(
                        _computerBattleAction ?? throw new Exception("No computer battle action set"),
                        battleData.ComputerBattler,
                        battleData.PlayerBattler,
                        BattleState.ComputerWins,
                        BattleState.PlayerWins,
                        BattleState.PlayerMainMenu
                    );
                    break;
            }
        }

        private void GenerateComputerBattleAction()
        {
            var battleData = _resourceService.Get<BattleData>("battle-data") ?? throw new Exception("Missing battle data");

            var selectedAttack = battleData.ComputerBattler.Attacks
                .Where(a => a.PowerPoints > 0)
                .OrderByDescending(a => a.Power)
                .FirstOrDefault();

            _computerBattleAction = new BattleAction(battleData.ComputerBattler, selectedAttack, null);

            SetBattleState(BattleState.ComputerAttackSelected);
        }

        private void ProcessTurn(BattleAction action, BattlerData source, BattlerData target, BattleState sourceWinState, BattleState targetWinState, BattleState nextState)
        {
            Thread.Sleep(1000);

            if (action == null)
            {
                SetBattleState(BattleState.Exception);
                return;
            }

            if (action.Attack != null)
            {
                target.Health -= action.Attack.Power;

                action.Attack.PowerPoints -= 1;
            }
            else if (action.Tool != null)
            {
                // process tool
                action.Tool.Count -= 1;
            }

            if (target.Health <= 0)
            {
                SetBattleState(sourceWinState);
            }
            else if (source.Health <= 0)
            {
                SetBattleState(targetWinState);
            }
            else
            {
                SetBattleState(nextState);
            }
        }

        private void OnAttackButtonPressed(object? sender, EventArgs e)
        {
            _battleState = BattleState.PlayerAttackMenu;
        }

        private void OnToolsButtonPressed(object? sender, EventArgs e)
        {
            _battleState = BattleState.PlayerToolsMenu;
        }

        private void OnRetreatButtonPressed(object? sender, EventArgs e)
        {
            _canRetreat = Random.Shared.Next() % 2 == 0;

            if (_canRetreat.Value)
            {
                _battleState = BattleState.PlayerRetreated;
            }
            else
            {
                _battleState = BattleState.PlayerMainMenu;
            }
        }

        private bool CanTryRetreat()
        {
            if (!_canRetreat.HasValue)
            {
                return true;
            }

            return _canRetreat.Value;
        }

        private bool? _canRetreat;

        private BattleAction? _playerBattleAction;
        private BattleAction? _computerBattleAction;

        private readonly IUtilityService _utilityService;
        private readonly InitializationArguments _initializationArguments;

        private enum BattleState
        {
            PlayerMainMenu,
            PlayerAttackMenu,
            PlayerAttackSelected,
            PlayerToolsMenu,
            PlayerToolSelected,
            PlayerRetreated,
            ComputerAttackSelected,
            PlayerExecutingAction,
            ComputerExecutingAction,
            PlayerWins,
            ComputerWins,
            Exception
        }

        private record BattleAction(BattlerData Battler, BattlerAttackData? Attack, BattlerToolData? Tool);
    }
}
