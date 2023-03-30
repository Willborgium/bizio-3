using Bizio.App.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Bizio.App.UI
{
    public abstract class ContainerBase : UiComponent, IContainer
    {
        public event EventHandler ChildrenChanged;

        public ContainerBase()
        {
            IsVisible = true;
        }

        public void AddChild(IIdentifiable child)
        {
            if (!CanAddChild(child))
            {
                throw new InvalidOperationException($"Cannot add child of type '{child?.GetType()}' to container of type '{GetType()}'.");
            }

            var didAddChild = false;

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

            if (child is ITranslatable t)
            {
                if (t.Parent != null)
                {
                    throw new InvalidOperationException("A translateable can only have one parent");
                }

                t.Parent = this;
            }

            if (child is IContainer c)
            {
                c.ChildrenChanged += OnNestedChildrenChanged;
            }

            _children.Add(child);

            Measure();

            ChildrenChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnNestedChildrenChanged(object sender, EventArgs e)
        {
            Measure();
        }

        protected virtual bool CanAddChild<T>(T child)
        {
            return true;
        }

        public void RemoveChild(IIdentifiable child)
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

            if (child is IContainer c)
            {
                c.ChildrenChanged -= OnNestedChildrenChanged;
            }

            Measure();

            ChildrenChanged?.Invoke(this, EventArgs.Empty);
        }

        public void RemoveChild(string identifier)
        {
            var child = FindChild(identifier);

            if (child == null)
            {
                return;
            }

            RemoveChild(child);
        }

        public IIdentifiable FindChild(string identifier) => FindChild<IIdentifiable>(identifier);

        public T FindChild<T>(string identifier)
        {
            foreach (var child in _children)
            {
                if (child.Identifier == identifier && child is T t)
                {
                    return t;
                }

                if (child is IContainer c)
                {
                    var result = c.FindChild<T>(identifier);

                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return default;
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

        public override void Update()
        {
            base.Update();

            var updateables = _updateables.ToList();

            foreach (var updateable in updateables)
            {
                updateable.Update();
            }
        }

        public abstract Vector2 GetChildAbsolutePosition(ITranslatable child);

        protected override void RenderInternal(SpriteBatch renderer)
        {
            foreach (var renderable in _renderables.OrderBy(r => r.ZIndex))
            {
                renderable.Render(renderer);
            }

            if (!_renderables.Any())
            {
                DebuggingService.DrawEmptyContainerText(renderer, GetCurrentPosition());
            }

            DebuggingService.DrawRectangle(renderer, GetCurrentPosition(), Dimensions);
        }

        protected Vector2 GetCurrentPosition()
        {
            return Parent?.GetChildAbsolutePosition(this) ?? Position;
        }

        protected override Vector2 GetDimensions() => _dimensions;

        private void Measure()
        {
            var currentPosition = GetCurrentPosition();
            float minX = currentPosition.X;
            float maxX = currentPosition.X;
            float minY = currentPosition.Y;
            float maxY = currentPosition.Y;

            foreach (var child in _children)
            {
                var position = currentPosition;

                if (child is ITranslatable t)
                {
                    position = GetChildAbsolutePosition(t);
                }

                if (minX > position.X)
                {
                    minX = position.X;
                }

                if (minY > position.Y)
                {
                    minY = position.Y;
                }

                if (child is IMeasurable m)
                {
                    var right = position.X + m.Dimensions.X;
                    var bottom = position.Y + m.Dimensions.Y;

                    if (right > maxX)
                    {
                        maxX = right;
                    }

                    if (bottom > maxY)
                    {
                        maxY = bottom;
                    }
                }
            }

            _dimensions = new Vector2(maxX - minX, maxY - minY);
        }

        public IEnumerator<IIdentifiable> GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        private Vector2 _dimensions;

        protected readonly ICollection<IIdentifiable> _children = new HashSet<IIdentifiable>();
        protected readonly ICollection<IRenderable> _renderables = new HashSet<IRenderable>();
        protected readonly ICollection<IUpdateable> _updateables = new HashSet<IUpdateable>();
    }
}
