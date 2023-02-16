using Microsoft.Xna.Framework;

namespace Bizio.App.UI
{
    public class VisualContainer : ContainerBase
    {
        public VisualContainer()
            : base()
        {
        }

        public override Vector2 GetChildAbsolutePosition(ITranslatable child)
        {
            return GetCurrentPosition() + child.Position;
        }
    }
}
