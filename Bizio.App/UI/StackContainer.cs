using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bizio.App.UI
{
    public class StackContainer : ContainerBase
    {
        public LayoutDirection Direction { get; set; }

        public Vector4 Padding { get; set; }

        public StackContainer()
            : base()
        {
        }

        public override Vector2 GetChildAbsolutePosition(ITranslatable child)
        {
            var position = GetCurrentPosition();

            position += new Vector2(Padding.X, Padding.Y) + child.Position;

            foreach (var renderable in _renderables)
            {
                if (renderable == child)
                {
                    return position;
                }

                if (!renderable.IsVisible)
                {
                    continue;
                }

                var measurable = renderable as IMeasurable;

                var offset = Vector2.Zero;

                switch (Direction)
                {
                    case LayoutDirection.Horizontal:
                        offset = new Vector2(measurable.Dimensions.X, 0) + new Vector2(Padding.Z, 0);
                        break;

                    case LayoutDirection.Vertical:
                        offset = new Vector2(0, measurable.Dimensions.Y) + new Vector2(0, Padding.W);
                        break;
                }

                position += offset;
            }

            return Vector2.Zero;
        }

        protected override bool CanAddChild<T>(T child)
        {
            if (child is not IMeasurable)
            {
                return false;
            }

            return base.CanAddChild(child);
        }
    }
}
