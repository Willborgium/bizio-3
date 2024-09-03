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

        public bool EnableDebugger { get; set; }

        public Sprite(ITexture2D texture)
        {
            _texture = texture;
            Scale = Vector2.One;
            Dimensions = new Vector2(texture.Width, texture.Height);
            IsVisible = true;
            _debugger = new SpriteDebugger(this);
            EnableDebugger = false;
        }

        public Sprite(ITexture2D texture, Rectangle source)
        {
            _texture = texture;
            Scale = Vector2.One;
            Dimensions = new Vector2(source.Width, source.Height);
            _source = source;
            IsVisible = true;
            _debugger = new SpriteDebugger(this);
            EnableDebugger = false;
        }

        public override void Update()
        {
            if (EnableDebugger)
            {
                _debugger.Update();
            }

            base.Update();
        }

        public void Render(IRenderer renderer)
        {
            if (!IsVisible)
            {
                return;
            }

            renderer.Draw(_texture, Position, Color.White, _source, Rotation, _absoluteOrigin, Scale);

            if (EnableDebugger)
            {
                _debugger.Render(renderer);
            }

            DebuggingService.Identify(renderer, this, Position);
            DebuggingService.DrawRectangle(renderer, Position, Dimensions * Scale, Rotation);
        }

        public Vector2 Translate(Vector2 offset) => Offset += offset;
        public Vector2 Translate(float x, float y) => Offset += new Vector2(x, y);
        public Vector2 Translate(int x, int y) => Offset += new Vector2(x, y);

        public bool Intersects(IBounded other)
        {
            var selfBounds = GetBounds();

            var otherBounds = other.GetBounds();

            if (selfBounds.Any(other.Contains) ||
                otherBounds.Any(Contains))
            {
                return true;
            }

            return false;
        }

        public IEnumerable<Vector2> GetBounds()
        {
            var key = new BoundsCacheKey(Position, Dimensions, Scale, Rotation, _relativeOrigin);

            if (_bounds.HasValue && _bounds.Value.Item1 == key)
            {
                return _bounds.Value.Item2;
            }

            var width = Dimensions.X * Scale.X;
            var height = Dimensions.Y * Scale.Y;

            var offsetX = _relativeOrigin.X * width;
            var offsetY = _relativeOrigin.Y * height;

            var topLeft = new Vector2(-offsetX, -offsetY);
            var topRight = new Vector2(width - offsetX, -offsetY);
            var bottomLeft = new Vector2(-offsetX, height - offsetY);
            var bottomRight = new Vector2(width - offsetX, height - offsetY);

            topLeft = RotatePoint(topLeft, Rotation);
            topRight = RotatePoint(topRight, Rotation);
            bottomLeft = RotatePoint(bottomLeft, Rotation);
            bottomRight = RotatePoint(bottomRight, Rotation);

            topLeft += Position;
            topRight += Position;
            bottomLeft += Position;
            bottomRight += Position;

            _bounds = (key, [topLeft, topRight, bottomLeft, bottomRight]);

            return _bounds.Value.Item2;
        }

        public bool Contains(Vector2 point)
        {
            // Step 1: Translate the point to the rectangle's local space
            var translatedPoint = point - Position;

            // Step 2: Reverse the rotation
            var cos = (float)Math.Cos(-Rotation);
            var sin = (float)Math.Sin(-Rotation);
            var rotatedPoint = new Vector2(
                translatedPoint.X * cos - translatedPoint.Y * sin,
                translatedPoint.X * sin + translatedPoint.Y * cos
            );

            // Step 3: Reverse the scale
            var scaledPoint = rotatedPoint / Scale;

            var x = (int)(-_relativeOrigin.X * Dimensions.X);
            var y = (int)(-_relativeOrigin.Y * Dimensions.Y);
            var width = (int)Dimensions.X;
            var height = (int)Dimensions.Y;

            // Step 4: Check if the point is within the bounds of the rectangle
            var localRect = new Rectangle(x, y, width, height);

            return localRect.Contains((int)scaledPoint.X, (int)scaledPoint.Y);
        }

        private (BoundsCacheKey, Vector2[])? _bounds;

        private static Vector2 RotatePoint(Vector2 point, float angle)
        {
            var cos = (float)Math.Cos(angle);
            var sin = (float)Math.Sin(angle);

            return new Vector2(
                point.X * cos - point.Y * sin,
                point.X * sin + point.Y * cos
            );
        }

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

        private static readonly Vector2 Half = Vector2.One / 2;

        private Vector2 _dimensions;
        private Vector2 _scale;
        private Vector2 _relativeOrigin;
        private Vector2 _absoluteOrigin;

        private RotationAnchor _anchor;

        private readonly ITexture2D _texture;
        private readonly Rectangle? _source;

        private readonly SpriteDebugger _debugger;

        private record BoundsCacheKey(Vector2 Position, Vector2 Dimensions, Vector2 Scale, float Rotation, Vector2 Origin);
    }
}
