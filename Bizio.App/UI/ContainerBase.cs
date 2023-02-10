using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Bizio.App.UI
{
    public abstract class ContainerBase : IContainer
    {
        public bool IsVisible { get; set; }

        public int ZIndex { get; set; }

        public IContainer Parent { get; set; }

        public Vector2 Position { get; set; }

        public ContainerBase()
        {
            IsVisible = true;
        }

        public void AddChild(IRenderable child)
        {
            if (!TryAddChild(child))
            {
                throw new InvalidOperationException($"Cannot add child of type '{child?.GetType()}' to container of type '{GetType()}'.");
            }

            _renderables.Add(child);
        }

        protected abstract bool TryAddChild(IRenderable child);

        public void RemoveChild(IRenderable child)
        {
            _renderables.Remove(child);

            if (child is ITranslatable t)
            {
                t.Parent = null;
            }
        }

        public abstract void Render(SpriteBatch renderer);

        public virtual Vector2 GetChildAbsolutePosition(ITranslatable child)
        {
            return Parent?.GetChildAbsolutePosition(child) ?? child.Position;
        }

        protected readonly ICollection<IRenderable> _renderables = new HashSet<IRenderable>();
    }
}
