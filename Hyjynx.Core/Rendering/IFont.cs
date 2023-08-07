using System.Numerics;

namespace Hyjynx.Core.Rendering
{
    public interface IFont
    {
        float LineSpacing { get; }
        Vector2 MeasureString(string text);
    }
}
