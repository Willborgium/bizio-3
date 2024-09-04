using System.Numerics;

namespace Hyjynx.Core.Rendering
{
    public class NoBounds : IBounded
    {
        public bool Contains(Vector2 point) => false;
        public IEnumerable<Vector2> GetBounds() => [];
        public bool Intersects(IBounded other) => false;

        public static readonly IBounded Instance = new NoBounds();
    }
}
