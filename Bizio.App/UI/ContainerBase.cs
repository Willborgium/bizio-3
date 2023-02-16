using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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

        public void AddChild<T>(T child)
        {
            if (!CanAddChild(child))
            {
                throw new InvalidOperationException($"Cannot add child of type '{child?.GetType()}' to container of type '{GetType()}'.");
            }

            var didAddChild = false;

            if (child is ITranslatable t)
            {
                if (t.Parent != null)
                {
                    throw new InvalidOperationException("A translateable can only have one parent");
                }

                t.Parent = this;
            }

            if (child is IRenderable r)
            {
                if (_renderables.Contains(r))
                {
                    throw new InvalidOperationException("A renderable can only belong to a container once");
                }

                didAddChild = true;

                _renderables.Add(r);
            }

            if (child is IUpdateable u)
            {
                if (_updateables.Contains(u))
                {
                    throw new InvalidOperationException("An updateable can only belong to a container once");
                }

                didAddChild = true;

                _updateables.Add(u);
            }

            if (!didAddChild)
            {
                throw new InvalidOperationException($"Did not add child of type '{child?.GetType()}' to container of type '{GetType()}' because it is neither renderable nor updateable");
            }

            _children.Add(child);
        }

        protected virtual bool CanAddChild<T>(T child)
        {
            return true;
        }

        public void RemoveChild<T>(T child)
        {
            if (child == null)
            {
                return;
            }

            if (!_children.Contains(child))
            {
                throw new InvalidOperationException("Object is not a child of this container");
            }

            _children.Remove(child);

            if (child is IUpdateable u)
            {
                _updateables.Remove(u);
            }

            if (child is IRenderable r)
            {
                _renderables.Remove(r);
            }

            if (child is ITranslatable t)
            {
                t.Parent = null;
            }
        }

        public ILocatable FindChild(string locator)
        {
            return (ILocatable)_children.FirstOrDefault(c => (c as ILocatable)?.Locator == locator);
        }

        public virtual void Render(SpriteBatch renderer)
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

        public int GetChildCount<T>(bool isRecursive)
        {
            var count = 0;

            foreach (var child in _children.Where(c => c is T))
            {
                if (isRecursive && child is IContainer container)
                {
                    count += container.GetChildCount<T>(true);
                }
                else
                {
                    count++;
                }
            }

            return count;
        }

        public int GetChildCount(bool isRecursive)
        {
            var count = 0;

            foreach (var child in _children)
            {
                if (isRecursive && child is IContainer container)
                {
                    count += container.GetChildCount(true);
                }
                else
                {
                    count++;
                }
            }

            return count;
        }

        public void Update()
        {
            var updateables = _updateables.ToList();

            foreach (var updateable in updateables)
            {
                updateable.Update();
            }
        }

        public abstract Vector2 GetChildAbsolutePosition(ITranslatable child);

        protected Vector2 GetCurrentPosition()
        {
            return Parent?.GetChildAbsolutePosition(this) ?? Position;
        }

        protected readonly ICollection<object> _children = new HashSet<object>();
        protected readonly ICollection<IRenderable> _renderables = new HashSet<IRenderable>();
        protected readonly ICollection<IUpdateable> _updateables = new HashSet<IUpdateable>();
    }
}
