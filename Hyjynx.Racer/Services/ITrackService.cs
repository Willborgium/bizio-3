using Hyjynx.Core.Rendering.Interface;
using Hyjynx.Racer.Models;

namespace Hyjynx.Racer.Services
{
    public interface ITrackService
    {
        IEnumerable<ITrackData> TrackMetadata { get; }

        VisualContainer CreateTrackContainer(ITrackData trackMetadata);
    }
}
