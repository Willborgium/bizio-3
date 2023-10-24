using System.Drawing;
using System.Numerics;

namespace Hyjynx.Core.Rendering
{
    public abstract class UiComponent : IRenderable, IUpdateable, IMeasurable
    {
        public bool IsVisible { get; set; }
        public int ZIndex { get; set; }
        public IContainer? Parent { get; set; }
        public Vector2 Offset { get => _offset + Parent?.ChildOffset ?? Vector2.Zero; set => _offset = value; } 
        public string? Identifier { get; set; }
        public ICollection<IUpdateable> Bindings { get; }
        public virtual Vector2 Dimensions { get => GetDimensions(); set => throw new InvalidOperationException("Dimensions are measured for this component and cannot be set directly."); }
        public virtual Rectangle Bounds
        {
            get
            {
                var x = (int)(Offset.X + Parent?.Bounds.X ?? 0 + Parent?.ChildOffset.X ?? 0);
                var y = (int)(Offset.Y + Parent?.Bounds.Y ?? 0 + Parent?.ChildOffset.Y ?? 0);

                return new Rectangle(x, y, (int)Dimensions.X, (int)Dimensions.Y);
            }
            set => throw new InvalidOperationException("Bounds are measured for this component and cannot be set directly.");
        }

        protected Vector2 _offset;

        public Color _background;

        protected UiComponent()
        {
            IsVisible = true;
            Bindings = new List<IUpdateable>();
            _background = RandomExtensions.RandomColor();
        }

        public void Rasterize(IRenderer renderer)
        {
            if (!IsAbsolutelyVisible())
            {
                return;
            }

            RasterizeInternal(renderer);
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
                return new Rectangle((int)Offset.X, (int)Offset.Y, (int)Dimensions.X, (int)Dimensions.Y);
            }
        }

        protected virtual void RasterizeInternal(IRenderer renderer) { }

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

            if (Dimensions.X <= 0 || Dimensions.Y <= 0)
            {
                return false;
            }

            return true;
        }

        private readonly IDictionary<string, object> _data = new Dictionary<string, object>();
    }
}
