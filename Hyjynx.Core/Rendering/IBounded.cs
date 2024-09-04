using System.Numerics;

namespace Hyjynx.Core.Rendering
{
    public interface IBounded
    {
        bool Contains(Vector2 point);
        bool Intersects(IBounded other);
        IEnumerable<Vector2> GetBounds();
    }
}
