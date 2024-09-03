using System.Numerics;

namespace Hyjynx.Core.Rendering
{
    public interface IBounded
    {
        bool Contains(Vector2 point);
        IEnumerable<Vector2> GetBounds();
    }
}
