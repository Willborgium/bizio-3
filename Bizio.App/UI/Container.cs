using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Bizio.App.UI
{
    public interface IContainer : IRenderable
    {
        void AddChild(IRenderable child);
        void RemoveChild(IRenderable child);
    }

    public abstract class ContainerBase : IContainer
    {
        public bool IsVisible { get; set; }

        public int ZIndex { get; set; }

        public void AddChild(IRenderable child)
        {
            _renderables.Add(child);
        }

        public void RemoveChild(IRenderable child)
        {
            _renderables.Remove(child);
        }

        public abstract void Render(SpriteBatch renderer);

        protected readonly ICollection<IRenderable> _renderables = new HashSet<IRenderable>();
    }

    // TODO: Fix this
    public class StackContainer : ContainerBase, IContainer
    {
        public override void Render(SpriteBatch renderer)
        {
            foreach (var renderable in _renderables)
            {
            }
        }
    }

    public class Container : ContainerBase, IContainer
    {
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
    }
}
