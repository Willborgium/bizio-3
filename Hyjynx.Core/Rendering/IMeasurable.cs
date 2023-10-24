using System.Drawing;
using System.Numerics;

namespace Hyjynx.Core.Rendering
{
    public interface IMeasurable : ITranslatable
    {
        public Vector2 Dimensions { get; }
        public Rectangle Bounds { get; }
    }
}
