using System.Numerics;

namespace Hyjynx.Core.Rendering
{
    public interface ITranslatable
    {
        // todo: gotta move this to some base class
        IContainer? Parent { get; set; }

        Vector2 Offset { get; set; }
        Vector2 Position { get; }

        Vector2 Translate(Vector2 offset);
        Vector2 Translate(float x, float y);
        Vector2 Translate(int x, int y);
    }
}
