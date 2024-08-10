using Hyjynx.Core.Rendering;
using Hyjynx.Core.Rendering.Interface;
using Hyjynx.Core.Services;
using System.Drawing;
using System.Numerics;

namespace Hyjynx.Racer.Scenes
{
    public class TestGameplayScene : Scene
    {
        public TestGameplayScene(
            IResourceService resourceService,
            IContentService contentService,
            ILoggingService loggingService,
            IInputService inputService
            )
            : base(
                  resourceService,
                  contentService,
                  loggingService
                  )
        {
            _inputService = inputService;
        }

        public override void LoadContent()
        {
            var statsPanel = new StackContainer
            {
                Direction = LayoutDirection.Vertical,
                Padding = new Vector4(5)                
            };

            _visualRoot.Add(statsPanel);

            var font = _resourceService.Get<IFont>("font-default");

            var velocity = new LabeledTextBox { Font = font, Color = Color.Black, Label = "Velocity" };
            statsPanel.AddChild(velocity);
            velocity.Bind(t => t.Text = $"{_velocity:0.000}");

            var carTexture = _contentService.Load<ITexture2D>("car-test");
            var car = new Sprite(carTexture)
            {
                Anchor = RotationAnchor.Center,
                Scale = new Vector2(0.5f, 0.5f),
                Position = new Vector2(500, 500)
            };

            _visualRoot.AddChild(car);

            car.Bind(TryMovePlayerCar);

            var rotation = new LabeledTextBox { Font = font, Color = Color.Black, Label = "Rotation" };
            statsPanel.AddChild(rotation);
            rotation.Bind(t => t.Text = $"{car.Rotation:0.000}");

            statsPanel.Bind(x => MoveStatsWithCar(x, car));
        }

        private static void MoveStatsWithCar(StackContainer container, Sprite car)
        {
            container.Position = car.Position - new Vector2(25, 200);
        }

        private void TryMovePlayerCar(Sprite sprite)
        {
            var w = _inputService.GetKeyState(Keys.W);
            var a = _inputService.GetKeyState(Keys.A);
            var s = _inputService.GetKeyState(Keys.S);
            var d = _inputService.GetKeyState(Keys.D);

            var acceleration = 0f;
            var rotation = 0f;

            if (w == KeyState.Down)
            {
                acceleration = ACCELERATION;
            }
            if (s == KeyState.Down)
            {
                acceleration = -ACCELERATION;
            }

            if (a == KeyState.Down)
            {
                rotation -= TURN_SPEED;
            }
            if (d == KeyState.Down)
            {
                rotation += TURN_SPEED;
            }

            if (acceleration != 0f)
            {
                _velocity += acceleration;
            }
            
            if (_velocity > 0.09f && acceleration <= 0)
            {
                _velocity -= FORWARD_DECELERATION;
            }
            
            if (_velocity < -0.09f && acceleration >= 0)
            {
                _velocity += REVERSE_DECELERATION;
            }

            if (acceleration == 0 &&
                _velocity < -0.11f &&
                _velocity > 0.11f)
            {
                _velocity = 0f;
            }

            if (_velocity == 0f)
            {
                return;
            }

            if (_velocity > TOP_FORWARD_SPEED)
            {
                _velocity = TOP_FORWARD_SPEED;
            }
            else if (_velocity < TOP_REVERSE_SPEED)
            {
                _velocity = TOP_REVERSE_SPEED;
            }

            float turnSpeedCoefficient;

            if (_velocity < 0)
            {
                turnSpeedCoefficient = _velocity / TOP_REVERSE_SPEED;
            }
            else
            {
                turnSpeedCoefficient = _velocity / TOP_FORWARD_SPEED;
            }

            if (rotation != 0f)
            {
                if (_velocity > 0f)
                {
                    sprite.Rotation += rotation * turnSpeedCoefficient;
                }
                else
                {
                    sprite.Rotation -= rotation * turnSpeedCoefficient;
                }
            }

            while (sprite.Rotation > TWO_PI)
            {
                sprite.Rotation -= TWO_PI;
            }

            while (sprite.Rotation < -TWO_PI)
            {
                sprite.Rotation += TWO_PI;
            }

            var r = sprite.Rotation;

            var x = (float)Math.Cos(r);
            var y = (float)Math.Sin(r);

            var translation = _velocity * new Vector2(x, y);

            sprite.Translate(translation);
        }

        private const float TWO_PI = (float)Math.PI * 2f;

        private const float ACCELERATION = .1f;
        private const float TOP_FORWARD_SPEED = 10f;
        private const float TOP_REVERSE_SPEED_FACTOR = .75f;
        private const float TOP_REVERSE_SPEED = TOP_FORWARD_SPEED * -TOP_REVERSE_SPEED_FACTOR;

        private const float FORWARD_DECELERATION_FACTOR = 3f;
        private const float FORWARD_DECELERATION = FORWARD_DECELERATION_FACTOR * ACCELERATION;

        private const float REVERSE_DECELERATION_FACTOR = 4f;
        private const float REVERSE_DECELERATION = REVERSE_DECELERATION_FACTOR * ACCELERATION;

        private const float TURN_SPEED = .03f;

        private readonly IInputService _inputService;

        private float _velocity;
    }
}
