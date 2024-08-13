using Hyjynx.Core;
using Hyjynx.Core.Rendering;
using Hyjynx.Core.Rendering.Interface;
using Hyjynx.Core.Services;
using Hyjynx.Racer.GameObjects;
using Newtonsoft.Json;
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
            IInputService inputService,
            InitializationArguments initializationArguments
            )
            : base(
                  resourceService,
                  contentService,
                  loggingService
                  )
        {
            _inputService = inputService;
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

            var playerVehicleData = new CalculatedVehicleData(.2f, 20f, .75f, 3f, .06f);
            var playerVehicle = new Vehicle(car, playerVehicleData)
            {
                GetInputs = GetPlayerInputs
            };

            _visualRoot += new VehicleDebugger(car, playerVehicle, Color.Blue);
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

        private readonly IInputService _inputService;
        private readonly InitializationArguments _initializationArguments;
        private IEnumerable<TrackData>? _trackData;
    }

    public class TrackData
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string TextureName { get; set; }

        public int TextureRows { get; set; }

        public int TextureColumns { get; set; }

        public TrackCell[,] Cells { get; set; }
    }

    public class TrackCell
    {
        public int TextureRow { get; set; }
        public int TextureColumn { get; set; }
    }
}
