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

            var battleData = _resourceService.Get<BattleData>("battle-data");

            var font = _resourceService.Get<IFont>("font-default");

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
                    case BattleState.PlayerExecutingAttackAction:
                        tb.Text = $"Player battler '{_playerBattleAction.Battler.Name}' executing attack '{_playerBattleAction.Attack.Name}'";
                        break;

                    case BattleState.ComputerExecutingAttackAction:
                        tb.Text = $"Player battler '{_computerBattleAction.Battler.Name}' executing attack '{_computerBattleAction.Attack.Name}'";
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
            };

            playerContainer.Position = new Vector2(0, _initializationArguments.ScreenHeight - playerContainer.Dimensions.Y);

            _visualRoot.AddChild(playerContainer);

            var aiStatsContainer = CreateBattleStatsContainer(battleData.ComputerBattler);

            _visualRoot.AddChild(aiStatsContainer);
        }

        private ContainerBase CreateBattleStatsContainer(BattlerData battler)
        {
            var font = _resourceService.Get<IFont>("font-default");

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
            container.AddChild(_utilityService.CreateButton("Tools", OnToolsButtonPressed));
            container.AddChild(_utilityService.CreateButton("Retreat", OnRetreatButtonPressed));

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

            foreach (var attack in battler.Attacks)
            {
                container.AddChild(_utilityService.CreateButton(attack.Name, (s, e) => OnPlayerAttackSelected(battler, attack)));
            }

            container.Bind(c => c.IsVisible = _battleState == BattleState.PlayerAttackMenu);

            return container;
        }

        private void OnPlayerAttackSelected(BattlerData battler, BattlerAttackData battlerAttack)
        {
            _playerBattleAction = new BattleAction(battler, battlerAttack);

            SetBattleState(BattleState.PlayerAttackSelected);
        }

        private void SetBattleState(BattleState battleState)
        {
            _battleState = battleState;

            switch (_battleState)
            {
                case BattleState.PlayerAttackSelected:
                case BattleState.PlayerToolSelected:
                    GenerateComputerBattleAction();
                    break;

                case BattleState.ComputerAttackSelected:
                    ProcessTurn();
                    break;

                case BattleState.PlayerExecutingAttackAction:
                case BattleState.ComputerExecutingAttackAction:
                    Thread.Sleep(1000);
                    break;
            }
        }

        private void GenerateComputerBattleAction()
        {
            var battleData = _resourceService.Get<BattleData>("battle-data");

            var selectedAttack = battleData.ComputerBattler.Attacks
                .Where(a => a.PowerPoints > 0)
                .OrderByDescending(a => a.Power)
                .FirstOrDefault();

            _computerBattleAction = new BattleAction(battleData.ComputerBattler, selectedAttack);

            SetBattleState(BattleState.ComputerAttackSelected);
        }

        private void ProcessTurn()
        {
            SetBattleState(BattleState.PlayerExecutingAttackAction);

            var battleData = _resourceService.Get<BattleData>("battle-data");

            battleData.ComputerBattler.Health -= _playerBattleAction.Attack.Power;

            _playerBattleAction.Attack.PowerPoints -= 1;

            if (battleData.ComputerBattler.Health <=0)
            {
                SetBattleState(BattleState.PlayerWins);
                return;
            }

            SetBattleState(BattleState.ComputerExecutingAttackAction);

            battleData.PlayerBattler.Health -= _computerBattleAction.Attack.Power;

            _computerBattleAction.Attack.PowerPoints -= 1;

            if (battleData.PlayerBattler.Health <= 0)
            {
                SetBattleState(BattleState.ComputerWins);
                return;
            }

            SetBattleState(BattleState.PlayerMainMenu);
        }

        private void OnAttackButtonPressed(object sender, EventArgs e)
        {
            _battleState = BattleState.PlayerAttackMenu;
        }

        private void OnToolsButtonPressed(object sender, EventArgs e)
        {
            _battleState = BattleState.PlayerToolsMenu;
        }

        private void OnRetreatButtonPressed(object sender, EventArgs e)
        {
            _battleState = BattleState.PlayerRetreatSelected;
        }

        private BattleAction _playerBattleAction;
        private BattleAction _computerBattleAction;

        private readonly IUtilityService _utilityService;
        private readonly InitializationArguments _initializationArguments;

        private enum BattleState
        {
            PlayerMainMenu,
            PlayerAttackMenu,
            PlayerAttackSelected,
            PlayerToolsMenu,
            PlayerToolSelected,
            PlayerRetreatSelected,
            ComputerAttackSelected,
            PlayerExecutingAttackAction,
            ComputerExecutingAttackAction,
            PlayerWins,
            ComputerWins
        }

        private record BattleAction(BattlerData Battler, BattlerAttackData? Attack);
    }
}
