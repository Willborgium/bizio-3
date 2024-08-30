using Hyjynx.Core.Debugging;
using System.Drawing;
using System.Numerics;

namespace Hyjynx.Core.Rendering
{
    public class Sprite : BindingBase, IRenderable, ITranslatable, IMeasurable, IRotatable
    {
        public bool IsVisible { get; set; }
        public int ZIndex { get; set; }
        public string? Identifier { get; set; }
        public IContainer? Parent { get; set; }
        public Vector2 Position { get; set; }

        public Vector2 Dimensions { get; set; }

        public Vector2 Scale { get; set; }

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
            get => _origin;
            set
            {
                if (Anchor != RotationAnchor.Custom)
                {
                    throw new InvalidOperationException($"Cannot explicitly set origin with Anchor set to '{Anchor}'. Please use type Custom.");
                }

                _origin = value;
            }
        }

        public Sprite(ITexture2D texture)
        {
            _texture = texture;
            Scale = Vector2.One;
            Dimensions = new Vector2(texture.Width, texture.Height);
            IsVisible = true;
        }

        public Sprite(ITexture2D texture, Rectangle source)
        {
            _texture = texture;
            Scale = Vector2.One;
            Dimensions = new Vector2(source.Width, source.Height);
            _source = source;
            IsVisible = true;
        }

        public void Render(IRenderer renderer)
        {
            if (!IsVisible)
            {
                return;
            }

            var position = Parent?.GetChildAbsolutePosition(this) ?? Position;

            renderer.Draw(_texture, position, Color.White, _source, Rotation, _origin, Scale);

            DebuggingService.Identify(renderer, this, position);
            DebuggingService.DrawRectangle(renderer, position, Dimensions * Scale, Rotation);
        }

        public Vector2 Translate(Vector2 offset) => Position += offset;
        public Vector2 Translate(float x, float y) => Position += new Vector2(x, y);
        public Vector2 Translate(int x, int y) => Position += new Vector2(x, y);

        public bool Intersects(Sprite other)
        {
            // Get the corners of the first rectangle
            var rect1Corners = GetTransformedCorners(Position - (Dimensions / 2), Dimensions, Scale, Rotation, Origin);

            // Get the corners of the second rectangle
            var rect2Corners = GetTransformedCorners(other.Position - (other.Dimensions / 2), other.Dimensions, other.Scale, other.Rotation, other.Origin);

            // Check if any of the corners of rect1 are inside rect2
            foreach (var corner in rect1Corners)
            {
                if (Contains(corner, other.Position - (other.Dimensions / 2), other.Rotation, other.Scale, other.Origin, other.Dimensions))
                {
                    return true;
                }
            }

            // Check if any of the corners of rect2 are inside rect1
            foreach (var corner in rect2Corners)
            {
                if (Contains(corner, (Position - Dimensions / 2), Rotation, Scale, Origin, Dimensions))
                {
                    return true;
                }
            }

            // No intersection found
            return false;
        }

        private static Vector2[] GetTransformedCorners(Vector2 position, Vector2 dimensions, Vector2 scale, float rotation, Vector2 origin)
        {
            Vector2[] corners = new Vector2[4];

            // Calculate the original corners relative to the origin
            corners[0] = new Vector2(-origin.X, -origin.Y); // Top-left
            corners[1] = new Vector2(dimensions.X - origin.X, -origin.Y); // Top-right
            corners[2] = new Vector2(dimensions.X - origin.X, dimensions.Y - origin.Y); // Bottom-right
            corners[3] = new Vector2(-origin.X, dimensions.Y - origin.Y); // Bottom-left

            // Apply scale
            for (int i = 0; i < 4; i++)
            {
                corners[i] *= scale;
            }

            // Apply rotation
            float cos = (float)Math.Cos(rotation);
            float sin = (float)Math.Sin(rotation);
            for (int i = 0; i < 4; i++)
            {
                float xNew = corners[i].X * cos - corners[i].Y * sin;
                float yNew = corners[i].X * sin + corners[i].Y * cos;
                corners[i] = new Vector2(xNew, yNew);
            }

            // Apply translation
            for (int i = 0; i < 4; i++)
            {
                corners[i] += position;
            }

            return corners;
        }

        private static bool Contains(Point point, Vector2 position, float rotation, Vector2 scale, Vector2 origin, Vector2 dimensions) =>
            Contains(new Vector2(point.X, point.Y), position, rotation, scale, origin, dimensions);

        private static bool Contains(Vector2 point, Vector2 position, float rotation, Vector2 scale, Vector2 origin, Vector2 dimensions)
        {
            // Step 1: Translate the point to the rectangle's local space
            var translatedPoint = point - position;

            // Step 2: Reverse the rotation
            var cos = (float)Math.Cos(-rotation);
            var sin = (float)Math.Sin(-rotation);
            var rotatedPoint = new Vector2(
                translatedPoint.X * cos - translatedPoint.Y * sin,
                translatedPoint.X * sin + translatedPoint.Y * cos
            );

            // Step 3: Reverse the scale
            var scaledPoint = rotatedPoint / scale;

            // Step 4: Check if the point is within the bounds of the rectangle
            var localRect = new Rectangle((int)-origin.X, (int)-origin.Y, (int)dimensions.X, (int)dimensions.Y);

            return localRect.Contains((int)scaledPoint.X, (int)scaledPoint.Y);
        }
        
        public bool Contains(Point point) => Contains(point, Position, Rotation, Scale, Origin, Dimensions);

        private void UpdateOrigin()
        {
            var width = Dimensions.X * Scale.X;
            var height = Dimensions.Y * Scale.Y;

            switch (Anchor)
            {
                case RotationAnchor.TopLeft:
                    _origin = new Vector2(0, 0);
                    break;
                case RotationAnchor.Center:
                    _origin = new Vector2(width / 2, height / 2);
                    break;
            }
        }

        private Vector2 _origin;
        private RotationAnchor _anchor;

        private readonly ITexture2D _texture;
        private readonly Rectangle? _source;
    }
}
