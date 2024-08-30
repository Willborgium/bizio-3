using Hyjynx.Core.Rendering.Interface;
using Hyjynx.Core.Rendering;
using Hyjynx.Core.Services;
using Hyjynx.Racer.Models;
using Newtonsoft.Json;
using System.Drawing;
using System.Numerics;

namespace Hyjynx.Racer.Services
{
    public class Track : VisualContainer
    {

    }

    public class TrackService : ITrackService
    {
        public IEnumerable<ITrackData> TrackMetadata
        {
            get
            {
                TryLoadTrackData();

                return _trackMetadata ?? Enumerable.Empty<ITrackData>();
            }
        }

        public TrackService(
            IResourceService resourceService,
            IContentService contentService
            )
        {
            _resourceService = resourceService;
            _contentService = contentService;
        }

        public Track CreateTrack(ITrackData trackMetadata)
        {
            var trackData = TrackData.First(td => td.Id == trackMetadata.Id);

            var trackTexture = _contentService.Load<ITexture2D>(trackData.TextureName);

            var cellWidth = trackTexture.Width / trackData.TextureColumns;
            var cellHeight = trackTexture.Height / trackData.TextureRows;

            var trackTextureSources = CreateTrackTextureSources(cellWidth, cellHeight, trackData);

            var trackContainer = new Track();

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
                        Anchor = RotationAnchor.Center,
                        Identifier = $"cell-{column}-{row}",
                        Position = new Vector2(x, y),
                        Scale = new Vector2(scale)
                    });
                }
            }

            return trackContainer;
        }

        private IEnumerable<TrackData> TrackData
        {
            get
            {
                TryLoadTrackData();

                return _trackData;
            }
        }

        private Rectangle[,] CreateTrackTextureSources(int cellWidth, int cellHeight, TrackData trackData)
        {
            var key = $"track-texture-sources-{trackData.Id}";

            var trackSources = _resourceService.Get<Rectangle[,]>(key);

            if (trackSources != null)
            {
                return trackSources;
            }

            trackSources = new Rectangle[trackData.TextureRows, trackData.TextureColumns];

            for (var row = 0; row < trackData.TextureRows; row++)
            {
                for (var column = 0; column < trackData.TextureColumns; column++)
                {
                    var x = column * cellWidth;
                    var y = row * cellHeight;

                    trackSources[row, column] = new Rectangle(x, y, cellWidth, cellHeight);
                }
            }

            _resourceService.Set(key, trackSources);

            return trackSources;
        }

        private void TryLoadTrackData()
        {
            if (_trackData != null) return;

            _trackData = LoadTrackData();
            _trackMetadata = _trackData.Cast<ITrackData>();

        }

        private static IEnumerable<TrackData> LoadTrackData()
        {
            string trackDataString;

            using (var reader = new StreamReader(TRACKS_FILE))
            {
                trackDataString = reader.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<IEnumerable<TrackData>>(trackDataString) ?? Enumerable.Empty<TrackData>();
        }

        private const string TRACKS_FILE = "tracks.json";

        private IEnumerable<TrackData>? _trackData;
        private IEnumerable<ITrackData>? _trackMetadata;

        private readonly IResourceService _resourceService;
        private readonly IContentService _contentService;
    }
}
