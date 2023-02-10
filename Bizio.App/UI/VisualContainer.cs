using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace Bizio.App.UI
{
    public class VisualContainer : ContainerBase, IContainer
    {
        public VisualContainer()
            : base()
        {
        }

        public override void Render(SpriteBatch renderer)
        {
            foreach (var renderable in _renderables.OrderBy(r => r.ZIndex))
            {
                if (!renderable.IsVisible)
                {
                    continue;
                }

                renderable.Render(renderer);
            }
        }

        public override Vector2 GetChildAbsolutePosition(ITranslatable child)
        {
            var position = Parent?.GetChildAbsolutePosition(this) ?? Position;

            return position + child.Position;
        }

        protected override bool TryAddChild(IRenderable child)
        {
            if (child is ITranslatable t)
            {
                t.Parent = this;
            }

            return true;
        }
    }
}
