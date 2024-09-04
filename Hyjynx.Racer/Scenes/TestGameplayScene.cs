using Hyjynx.Core;
using Hyjynx.Core.Debugging;
using Hyjynx.Core.Rendering;
using Hyjynx.Core.Rendering.Interface;
using Hyjynx.Core.Services;
using Hyjynx.Racer.GameObjects;
using Hyjynx.Racer.Models;
using Hyjynx.Racer.Services;
using System.Drawing;
using System.Numerics;
using System.Xml.Xsl;

namespace Hyjynx.Racer.Scenes
{
    public class TestGameplayScene : Scene
    {
        public TestGameplayScene(
            IResourceService resourceService,
            IContentService contentService,
            ILoggingService loggingService,
            IInputService inputService,
            IUtilityService utilityService,
            ITrackService trackService,
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
            _trackService = trackService;
            _initializationArguments = initializationArguments;
        }

        public override void LoadContent()
        {
            _visualRoot += DebuggingService.CreateDebugContainer(_loggingService, _utilityService, _visualRoot, _initializationArguments);

            var tb = new TextBox { Font = DebuggingService.Font, Color = Color.Red, ZIndex = 5 };
            _visualRoot += tb;
            tb.Bind(t =>
            {
                var m = _inputService.GetMouseState();
                t.Offset = new Vector2(m.Position.X, m.Position.Y);
                t.Text = $"{m.Position}";
            });

            // CAR
            var carTexture = _contentService.Load<ITexture2D>("car-test");
            var car = new Sprite(carTexture)
            {
                Anchor = RotationAnchor.Center,
                Scale = new Vector2(0.5f, 0.5f),
                Offset = new Vector2(500, 500),
                Rotation = .5f,
                ZIndex = 1,
                Identifier = "car"
            };

            car.Debugger.IsEnabled = true;

            car.Bounds = new RectangleBounds(car);

            _visualRoot += car;

            var vehicleDataController = new VehicleDataController(_utilityService);
            _visualRoot += vehicleDataController;

            var playerVehicleData = new DynamicVehicleData(
                () => vehicleDataController.Acceleration,
                () => vehicleDataController.TopSpeed,
                () => vehicleDataController.TopReverseSpeedFactor,
                () => vehicleDataController.DecelarationFactor,
                () => vehicleDataController.TurnSpeed
            );


            var playerVehicle = new Vehicle(car, playerVehicleData)
            {
                GetInputs = GetPlayerInputs
            };

            //_visualRoot += new VehicleDebugger(car, playerVehicle, Color.Blue);

            car.Debugger.AddLabel("Velocity", () => VisualDebugger.Print(playerVehicle.Velocity));

            // TRACK SELECTOR
            var tracks = _trackService.TrackMetadata;

            var trackSelectorContainer = new StackContainer
            {
                Direction = LayoutDirection.Vertical,
                Padding = new Vector4(5),
                ZIndex = 0
            };

            _visualRoot += trackSelectorContainer;

            foreach (var track in tracks)
            {
                trackSelectorContainer.Add(_utilityService.CreateButton(track.Name, (s, e) => SelectTrack(trackSelectorContainer, track, car)));
            }

            vehicleDataController.Bind(c => c.IsVisible = !trackSelectorContainer.IsVisible);
        }

        private void SelectTrack(IRenderable container, ITrackData track, Sprite car)
        {
            var trackContainer = _trackService.CreateTrack(track);

            _visualRoot += trackContainer;

            trackContainer.Bind(t => UpdateViewport(t, car));

            container.IsVisible = false;
        }

        private void UpdateViewport(VisualContainer track, Sprite car)
        {
            var xOffset = 0f;
            var yOffset = 0f;

            var boundaryThickness = 400;

            var leftBound = boundaryThickness;
            var rightBound = _initializationArguments.ScreenWidth - boundaryThickness;

            var topBound = boundaryThickness;
            var bottomBound = _initializationArguments.ScreenHeight - boundaryThickness;

            if (car.Offset.X < leftBound)
            {
                xOffset = leftBound - car.Offset.X;
            }
            else if (car.Offset.X > rightBound)
            {
                xOffset = rightBound - car.Offset.X;
            }

            if (car.Offset.Y < topBound)
            {
                yOffset = topBound - car.Offset.Y;
            }
            else if (car.Offset.Y > bottomBound)
            {
                yOffset = bottomBound - car.Offset.Y;
            }

            car.Translate(xOffset, yOffset);
            track.Translate(xOffset, yOffset);
        }

        private VehicleInputs GetPlayerInputs()
        {
            var w = _inputService.GetKeyState(Keys.W);
            var a = _inputService.GetKeyState(Keys.A);
            var s = _inputService.GetKeyState(Keys.S);
            var d = _inputService.GetKeyState(Keys.D);

            return new VehicleInputs(
                w == KeyState.Down,
                s == KeyState.Down,
                a == KeyState.Down,
                d == KeyState.Down
            );
        }

        private readonly IInputService _inputService;
        private readonly IUtilityService _utilityService;
        private readonly ITrackService _trackService;
        private readonly InitializationArguments _initializationArguments;
    }
}
