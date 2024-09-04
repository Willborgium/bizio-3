using Hyjynx.Core.Debugging;
using System.Drawing;
using System.Numerics;

namespace Hyjynx.Core.Rendering
{
    public class Sprite : BindingBase, IRenderable, ITranslatable, IMeasurable, IRotatable, IBounded
    {
        public bool IsVisible { get; set; }
        public int ZIndex { get; set; }
        public string? Identifier { get; set; }
        public IContainer? Parent { get; set; }
        public Vector2 Offset { get; set; }

        public Vector2 Position => Parent?.GetChildAbsolutePosition(this) ?? Offset;

        public Vector2 Dimensions
        {
            get => _dimensions;
            set
            {
                _dimensions = value;
                UpdateOrigin();
            }
        }

        public Vector2 Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                UpdateOrigin();
            }
        }

        public float Rotation { get; set; }

        public RotationAnchor Anchor
        {
            get => _anchor;
            set
            {
                _anchor = value;
                UpdateOrigin();
            }
        }

        public Vector2 Origin
        {
            get => _absoluteOrigin;
            set
            {
                if (Anchor != RotationAnchor.Custom)
                {
                    throw new InvalidOperationException($"Cannot explicitly set origin with Anchor set to '{Anchor}'. Please use type Custom.");
                }

                _absoluteOrigin = value;
            }
        }

        public Vector2 RelativeOrigin => _relativeOrigin;

        public IDebugger Debugger { get; }

        public IBounded Bounds { get; set; }

        public Sprite(ITexture2D texture, Rectangle? source = null, bool enableDebugger = false, IBounded? bounds = null)
        {
            Scale = Vector2.One;
            IsVisible = true;

            _texture = texture;

            if (source != null)
            {
                Dimensions = new Vector2(source.Value.Width, source.Value.Height);
                _source = source;
            }
            else
            {
                Dimensions = new Vector2(texture.Width, texture.Height);
            }

            Debugger = new SpriteDebugger(this)
            {
                IsEnabled = enableDebugger
            };

            Bounds = bounds ?? NoBounds.Instance;
        }

        public void Render(IRenderer renderer)
        {
            if (!IsVisible)
            {
                return;
            }

            renderer.Draw(_texture, Position, Color.White, _source, Rotation, _absoluteOrigin, Scale);

            Debugger.Render(renderer);

            DebuggingService.Identify(renderer, this, Position);
            DebuggingService.DrawRectangle(renderer, Position, Dimensions * Scale, Rotation);
        }

        public Vector2 Translate(Vector2 offset) => Offset += offset;
        public Vector2 Translate(float x, float y) => Offset += new Vector2(x, y);
        public Vector2 Translate(int x, int y) => Offset += new Vector2(x, y);

        private void UpdateOrigin()
        {
            var width = Dimensions.X;
            var height = Dimensions.Y;

            switch (Anchor)
            {
                case RotationAnchor.TopLeft:
                    _relativeOrigin = Vector2.Zero;
                    _absoluteOrigin = Vector2.Zero;
                    break;

                case RotationAnchor.Center:
                    _relativeOrigin = Half;
                    _absoluteOrigin = new Vector2(width / 2, height / 2);
                    break;
            }
        }

        public bool Contains(Vector2 point) => Bounds.Contains(point);

        public bool Intersects(IBounded other) => Bounds.Intersects(other);

        public IEnumerable<Vector2> GetBounds() => Bounds.GetBounds();

        private static readonly Vector2 Half = Vector2.One / 2;

        private Vector2 _dimensions;
        private Vector2 _scale;
        private Vector2 _relativeOrigin;
        private Vector2 _absoluteOrigin;

        private RotationAnchor _anchor;

        private readonly ITexture2D _texture;
        private readonly Rectangle? _source;
    }
}
