﻿using System.Drawing;
using System.Numerics;

namespace Hyjynx.Core.Rendering
{
    public abstract class UiComponent : IRenderable, IUpdateable, ITranslatable, IMeasurable
    {
        public bool IsVisible { get; set; }
        public int ZIndex { get; set; }
        public IContainer? Parent { get; set; }
        public Vector2 Position { get; set; }
        public string? Identifier { get; set; }
        public ICollection<IUpdateable> Bindings { get; }
        public virtual Vector2 Dimensions { get => GetDimensions(); set => throw new InvalidOperationException("Dimensions are measured for this component and cannot be set directly."); }

        protected UiComponent()
        {
            IsVisible = true;
            Bindings = new List<IUpdateable>();
        }

        public void Render(IRenderer renderer)
        {
            if (!IsAbsolutelyVisible())
            {
                return;
            }

            RenderInternal(renderer);
        }

        public virtual void Update()
        {
            foreach (var binding in Bindings)
            {
                binding.Update();
            }
        }

        public void SetData(string key, object data) { _data[key] = data; }

        public T GetData<T>(string key)
        {
            if (!_data.ContainsKey(key))
            {
                return default;
            }

            return (T)_data[key];
        }

        protected Rectangle Destination
        {
            get
            {
                var position = Parent?.GetChildAbsolutePosition(this) ?? Position;

                return new Rectangle((int)position.X, (int)position.Y, (int)Dimensions.X, (int)Dimensions.Y);
            }
        }

        protected abstract void RenderInternal(IRenderer renderer);

        protected abstract Vector2 GetDimensions();

        private bool IsAbsolutelyVisible()
        {
            if (!IsVisible)
            {
                return false;
            }

            var parent = Parent;

            while (parent != null)
            {
                if (!parent.IsVisible)
                {
                    return false;
                }

                parent = parent.Parent;
            }

            return true;
        }

        private readonly IDictionary<string, object> _data = new Dictionary<string, object>();
    }
}
