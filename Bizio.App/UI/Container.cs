using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bizio.App.UI
{
    public interface IContainer : IRenderable, ITranslatable
    {
        void AddChild(IRenderable child);
        void RemoveChild(IRenderable child);
    }

    public abstract class ContainerBase : IContainer
    {
        public bool IsVisible { get; set; }

        public int ZIndex { get; set; }

        public ITranslatable Parent { get; set; }

        public Vector2 Position { get; set; }

        public void AddChild(IRenderable child)
        {
            _renderables.Add(child);

            if (child is ITranslatable t)
            {
                t.Parent = this;
            }
        }

        public void RemoveChild(IRenderable child)
        {
            _renderables.Remove(child);

            if (child is ITranslatable t)
            {
                t.Parent = null;
            }
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
