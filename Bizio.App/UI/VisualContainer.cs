using Microsoft.Xna.Framework;

namespace Bizio.App.UI
{
    public class VisualContainer : ContainerBase
    {
        public override Vector2 GetChildAbsolutePosition(ITranslatable child)
        {
            return GetCurrentPosition() + child.Position;
        }
    }
}
