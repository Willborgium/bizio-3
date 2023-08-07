using System.Numerics;

namespace Hyjynx.Core.Rendering.Interface
{
    public class VisualContainer : ContainerBase
    {
        public override Vector2 GetChildAbsolutePosition(ITranslatable child)
        {
            return GetCurrentPosition() + child.Position;
        }
    }
}
