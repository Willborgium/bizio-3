using Hyjynx.Core.Rendering;
using Hyjynx.Core.Rendering.Interface;
using Hyjynx.Core.Services;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Hyjynx.Tycoon.Scenes
{
    internal class GameScene : Scene
    {
        public GameScene(
            IResourceService resourceService,
            IContentService contentService,
            ILoggingService loggingService,
            IUtilityService utilityService,
            IInputService inputService
            )
            : base(
                  resourceService,
                  contentService,
                  loggingService
                  )
        {
            _utilityService = utilityService;
            _inputService = inputService;
        }

        public override void LoadContent()
        {
            _loggingService.Info("Game started");

            _utilityService.TryAddDebuggingContainer(_visualRoot);

            var carTexture = _contentService.Load<ITexture2D>("car-test");
            var carSprite = new Sprite(carTexture)
            {
                Position = new Vector2(250, 250),
                Anchor = RotationAnchor.Custom,
                Scale = new Vector2(.5f, .5f),
                Origin = new Vector2(100, 104)
            };
            _resourceService.Set("player-car", carSprite);

            _inputService.KeyStatesChanged += OnKeyStatesChanged;

            carSprite.Bind(TryMovePlayerCar);

            _visualRoot.AddChild(carSprite);

            var font = _resourceService.Get<IFont>("font-default") ?? throw new Exception();

            var statsStacker = new StackContainer
            {
                Position = new Vector2(1000, 0),
                Padding = new Vector4(10, 10, 10, 10)
            };
            _visualRoot.AddChild(statsStacker);

            void Describe(string label, Func<string> descriptor)
            {
                var tb = new LabeledTextBox
                {
                    Color = Color.Black,
                    Font = font,
                    Label = label
                };
                tb.Bind(x => x.Text = descriptor());
                statsStacker.Add(tb);
            }

            Describe("Acceleration", () => $"{_acceleration:0.00}");
            Describe("Velocity", () =>  $"{_velocity:0.00}");
            Describe("Velocity Factor", () => $"{_velocityFactor:0.00}");
            Describe("Rotation RoC", () => $"{_rotationRateOfChange:0.0000}");
            Describe("Rotation D2", () => $"{_rotationD2:0.0000}");
            Describe("Rotation D1", () => $"{_rotationD1:0.0000}");
            Describe("Rotation", () => $"{carSprite.Rotation:0.0000}");

            base.LoadContent();
        }

        private const float ROTATION_FACTOR = INCREMENT / 3f;
        private const float ROTATION_D2_FACTOR = ROTATION_FACTOR / 10f;
        private const float ROTATION_D1_FACTOR = ROTATION_D2_FACTOR * 2f;

        private void OnKeyStatesChanged(object? sender, KeyStatesChangedEventArgs e)
        {
            _acceleration = MapKeyToValue(e.KeyStates[Keys.W], e.KeyStates[Keys.S], INCREMENT);
            _rotationRateOfChange = MapKeyToValue(e.KeyStates[Keys.D], e.KeyStates[Keys.A], ROTATION_FACTOR);

            _rotationD2 = MapKeyToValue(e.KeyStates[Keys.D], e.KeyStates[Keys.A], ROTATION_D2_FACTOR);
        }

        private float MapKeyToValue(KeyState increaseKeyState, KeyState decreaseKeyState, float value)
        {
            var output = 0f;

            if (decreaseKeyState == KeyState.Pressed ||
                decreaseKeyState == KeyState.Down)
            {
                if (increaseKeyState == KeyState.Up ||
                    increaseKeyState == KeyState.Released)
                {
                    output = -value;
                }
                else
                {
                    output = 0;
                }
            }
            else if (increaseKeyState == KeyState.Pressed ||
                increaseKeyState == KeyState.Down)
            {
                if (decreaseKeyState == KeyState.Up ||
                    decreaseKeyState == KeyState.Released)
                {
                    output = value;
                }
                else
                {
                    output = 0;
                }
            }

            return output;
        }

        private float SmoothReduce(float value, float clamp)
        {
            var output = value * .9f;

            if (Math.Abs(output) < clamp)
            {
                output = 0;
            }

            return output;
        }

        private void TryMovePlayerCar(Sprite playerCar)
        {
            if (_acceleration != 0)
            {
                _velocity += _acceleration;

                _velocity = Math.Clamp(_velocity, -VELOCITY_LIMIT, VELOCITY_LIMIT);
            }
            else
            {
                _velocity = SmoothReduce(_velocity, INCREMENT);
            }

            _velocityFactor = Math.Abs(_velocity) / VELOCITY_LIMIT;

            if (_rotationD2 != 0)
            {
                _rotationD1 += _rotationD2;

                _rotationD1 = Math.Clamp(_rotationD1, -ROTATION_FACTOR, ROTATION_FACTOR);
            }
            else
            {
                _rotationD1 = SmoothReduce(_rotationD1, ROTATION_D1_FACTOR);
            }

            var rotation = _rotationD1 * _velocityFactor;

            if (_velocity > 0)
            {
                playerCar.Rotation += rotation;
            }
            else if (_velocity < 0)
            {
                playerCar.Rotation -= rotation;
            }

            playerCar.Translate((float)Math.Cos(playerCar.Rotation) * _velocity, (float)Math.Sin(playerCar.Rotation) * _velocity);
        }

        private const float INCREMENT = .25f;
        private const float VELOCITY_LIMIT = 15f;

        private float _acceleration;
        private float _velocity;

        private float _rotationD2;
        private float _rotationD1;

        private float _rotationRateOfChange;
        private float _velocityFactor;

        private readonly IUtilityService _utilityService;
        private readonly IInputService _inputService;
    }
}
