using Hyjynx.Core.Rendering;
using Hyjynx.Core.Services;
using Hyjynx.Racer.GameObjects;
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
            var carTexture = _contentService.Load<ITexture2D>("car-test");
            var car = new Sprite(carTexture)
            {
                Anchor = RotationAnchor.Center,
                Scale = new Vector2(0.5f, 0.5f),
                Position = new Vector2(500, 500)
            };

            _visualRoot.AddChild(car);

            var playerVehicleData = new CalculatedVehicleData(.1f, 10f, .75f, 3f, .03f);
            var playerVehicle = new Vehicle(car, playerVehicleData)
            {
                GetInputs = GetPlayerInputs
            };

            var debugger = new VehicleDebugger(car, playerVehicle);

            _visualRoot.Add(debugger);
        }

        public override void UnloadContent()
        {
            _resourceService.Set("player-vehicle", null);
        }

        private VehicleInputs GetPlayerInputs()
        {
            var w = _inputService.GetKeyState(Keys.W);
            var a = _inputService.GetKeyState(Keys.A);
            var s = _inputService.GetKeyState(Keys.S);
            var d = _inputService.GetKeyState(Keys.D);

            return new VehicleInputs
            {
                Accelerate = w == KeyState.Down,
                Brake = s == KeyState.Down,
                TurnLeft = a == KeyState.Down,
                TurnRight = d == KeyState.Down
            };
        }

        private readonly IInputService _inputService;
    }
}
