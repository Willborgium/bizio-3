using System.Drawing;
using System.Numerics;

namespace Hyjynx.Core.Rendering
{
    public abstract class UiComponent : BindingBase, IRenderable, IUpdateable, ITranslatable, IMeasurable
    {
        public bool IsVisible { get; set; }
        public int ZIndex { get; set; }
        public IContainer? Parent { get; set; }
        public Vector2 Offset { get; set; }
        public Vector2 Position => Parent?.GetChildAbsolutePosition(this) ?? Offset;
        public string? Identifier { get; set; }
        public virtual Vector2 Dimensions { get => GetDimensions(); set => throw new InvalidOperationException("Dimensions are measured for this component and cannot be set directly."); }

        protected UiComponent()
            : base()
        {
            IsVisible = true;
        }

        public void Render(IRenderer renderer)
        {
            if (!IsAbsolutelyVisible())
            {
                return;
            }

            RenderInternal(renderer);
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

        public Vector2 Translate(Vector2 offset) => Offset += offset;
        public Vector2 Translate(float x, float y) => Offset += new Vector2(x, y);
        public Vector2 Translate(int x, int y) => Offset += new Vector2(x, y);

        protected Rectangle Destination
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, (int)Dimensions.X, (int)Dimensions.Y);
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
