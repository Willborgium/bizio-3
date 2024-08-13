using Hyjynx.Core;
using Hyjynx.Core.Debugging;
using Hyjynx.Core.Rendering;
using Hyjynx.Core.Rendering.Interface;
using Hyjynx.Core.Services;
using Hyjynx.Racer.GameObjects;
using Hyjynx.Racer.Models;
using Newtonsoft.Json;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;

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
            _initializationArguments = initializationArguments;
        }

        public override void LoadContent()
        {
            _trackData = LoadTrackData();

            var track = CreateTrackContainer(_trackData.First());

            _visualRoot += track;

            var carTexture = _contentService.Load<ITexture2D>("car-test");
            var car = new Sprite(carTexture)
            {
                Anchor = RotationAnchor.Center,
                Scale = new Vector2(0.5f, 0.5f),
                Position = new Vector2(500, 500)
            };

            _visualRoot += car;

            track.Bind(t => UpdateViewport(t, car));

            var playerVehicleData = new DynamicVehicleData(
                () => _acceleration,
                () => _topSpeed,
                () => _topReverseSpeedFactor,
                () => _decelarationFactor,
                () => _turnSpeed
            );

            var playerVehicle = new Vehicle(car, playerVehicleData)
            {
                GetInputs = GetPlayerInputs
            };

            _visualRoot += new VehicleDebugger(car, playerVehicle, Color.Blue);

            IContainer vehicleDataControllerContainer = new StackContainer
            {
                Direction = LayoutDirection.Vertical,
                Padding = new Vector4(5)
            };

            vehicleDataControllerContainer += AddVehicleDataControllerButtons("Acceleration", () => _acceleration, x => _acceleration = x, .01f);
            vehicleDataControllerContainer += AddVehicleDataControllerButtons("Top Speed", () => _topSpeed, x => _topSpeed = x, 1f);
            vehicleDataControllerContainer += AddVehicleDataControllerButtons("Reverse Factor", () => _topReverseSpeedFactor, x => _topReverseSpeedFactor = x, .05f);
            vehicleDataControllerContainer += AddVehicleDataControllerButtons("Decelaration Factor", () => _decelarationFactor, x => _decelarationFactor = x, .25f);
            vehicleDataControllerContainer += AddVehicleDataControllerButtons("Turn Speed", () => _turnSpeed, x => _turnSpeed = x, .01f);

            _visualRoot += vehicleDataControllerContainer;
        }

        private IContainer AddVehicleDataControllerButtons(string name, Func<float> getter, Action<float> setter, float increment)
        {
            IContainer container = new StackContainer
            {
                Direction = LayoutDirection.Horizontal,
                Padding = new Vector4(5)
            };

            container += new TextBox { Font = DebuggingService.Font, MinimumDimensions = new Vector2(200, 50), Color = Color.Black, Text = name };
            container += _utilityService.CreateButton("-", 50, ChangeValue(getter, setter, -increment));
            container += _utilityService.CreateButton("+", 50, ChangeValue(getter, setter, increment));
            var value = new TextBox { Font = DebuggingService.Font, Color = Color.Black };
            value.Bind(v => v.Text = $"{getter():0.000}");
            container += value;

            return container;
        }

        private static EventHandler ChangeValue(Func<float> getter, Action<float> setter, float increment)
        {
            return (s, e) =>
            {
                setter(getter() + increment);
            };
        }

        private float _acceleration = .1f;
        private float _topSpeed = 10f;
        private float _topReverseSpeedFactor = .75f;
        private float _decelarationFactor = 1.5f;
        private float _turnSpeed = .03f;

        private void UpdateViewport(VisualContainer track, Sprite car)
        {
            var xOffset = 0f;
            var yOffset = 0f;

            var boundaryThickness = 400;

            var leftBound = boundaryThickness;
            var rightBound = _initializationArguments.ScreenWidth - boundaryThickness;

            var topBound = boundaryThickness;
            var bottomBound = _initializationArguments.ScreenHeight - boundaryThickness;

            if (car.Position.X < leftBound)
            {
                xOffset = leftBound - car.Position.X;
            }
            else if (car.Position.X > rightBound)
            {
                xOffset = rightBound - car.Position.X;
            }

            if (car.Position.Y < topBound)
            {
                yOffset = topBound - car.Position.Y;
            }
            else if (car.Position.Y > bottomBound)
            {
                yOffset = bottomBound - car.Position.Y;
            }

            car.Translate(xOffset, yOffset);
            track.Translate(xOffset, yOffset);
        }

        private IEnumerable<TrackData> LoadTrackData()
        {
            string trackDataString;

            using (var reader = new StreamReader("tracks.json"))
            {
                trackDataString = reader.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<IEnumerable<TrackData>>(trackDataString);
        }

        private Rectangle[,] CreateTrackTextureSources(int cellWidth, int cellHeight, TrackData trackData)
        {
            var trackSources = new Rectangle[trackData.TextureRows, trackData.TextureColumns];

            for (var row = 0; row < trackData.TextureRows; row++)
            {
                for (var column = 0; column < trackData.TextureColumns; column++)
                {
                    var x = column * cellWidth;
                    var y = row * cellHeight;

                    trackSources[row, column] = new Rectangle(x, y, cellWidth, cellHeight);
                }
            }

            return trackSources;
        }

        private VisualContainer CreateTrackContainer(TrackData trackData)
        {
            var trackTexture = _contentService.Load<ITexture2D>(trackData.TextureName);

            var cellWidth = trackTexture.Width / trackData.TextureColumns;
            var cellHeight = trackTexture.Height / trackData.TextureRows;

            var trackTextureSources = CreateTrackTextureSources(cellWidth, cellHeight, trackData);

            var trackContainer = new VisualContainer();

            var rowCount = trackData.Cells.GetLength(0);
            var columnCount = trackData.Cells.GetLength(1);

            var scale = 2f;

            for (var row = 0; row < rowCount; row++)
            {
                for (var column = 0; column < columnCount; column++)
                {
                    var x = column * cellWidth * scale;
                    var y = row * cellHeight * scale;

                    var cellData = trackData.Cells[row, column];

                    var source = trackTextureSources[cellData.TextureRow, cellData.TextureColumn];

                    trackContainer.AddChild(new Sprite(trackTexture, source)
                    {
                        Position = new Vector2(x, y),
                        Scale = new Vector2(scale)
                    });
                }
            }

            return trackContainer;
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

        private IEnumerable<TrackData>? _trackData;

        private readonly IInputService _inputService;
        private readonly IUtilityService _utilityService;
        private readonly InitializationArguments _initializationArguments;
    }
}
