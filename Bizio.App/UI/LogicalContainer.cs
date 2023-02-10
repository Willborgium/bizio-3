using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace Bizio.App.UI
{
    public class LogicalContainer : ContainerBase, IContainer
    {
        public override void Render(SpriteBatch renderer)
        {
            foreach (var renderable in _renderables.OrderBy(r => r.ZIndex))
            {
                renderable.Render(renderer);
            }
        }

        protected override bool TryAddChild(IRenderable child)
        {
            return true;
        }
    }
}
