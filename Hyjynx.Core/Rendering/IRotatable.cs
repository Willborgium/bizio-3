using System.Numerics;

namespace Hyjynx.Core.Rendering
{
    public enum RotationAnchor
    {
        TopLeft,
        Center,
        Custom
    }

    public interface IRotatable
    {
        float Rotation { get; set; }
        RotationAnchor Anchor { get; set; }
        Vector2 Origin { get; set; }
    }
}
