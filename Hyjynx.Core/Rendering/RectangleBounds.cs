using System.Drawing;
using System.Numerics;

namespace Hyjynx.Core.Rendering
{
    public class RectangleBounds(Sprite target) : IBounded
    {
        private readonly Sprite _target = target;

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
            var key = new BoundsCacheKey(_target.Position, _target.Dimensions, _target.Scale, _target.Rotation, _target.RelativeOrigin);

            if (_bounds.HasValue && _bounds.Value.Item1 == key)
            {
                return _bounds.Value.Item2;
            }

            var width = _target.Dimensions.X * _target.Scale.X;
            var height = _target.Dimensions.Y * _target.Scale.Y;

            var offsetX = _target.RelativeOrigin.X * width;
            var offsetY = _target.RelativeOrigin.Y * height;

            var topLeft = new Vector2(-offsetX, -offsetY);
            var topRight = new Vector2(width - offsetX, -offsetY);
            var bottomLeft = new Vector2(-offsetX, height - offsetY);
            var bottomRight = new Vector2(width - offsetX, height - offsetY);

            topLeft = RotatePoint(topLeft, _target.Rotation);
            topRight = RotatePoint(topRight, _target.Rotation);
            bottomLeft = RotatePoint(bottomLeft, _target.Rotation);
            bottomRight = RotatePoint(bottomRight, _target.Rotation);

            topLeft += _target.Position;
            topRight += _target.Position;
            bottomLeft += _target.Position;
            bottomRight += _target.Position;

            _bounds = (key, [topLeft, topRight, bottomLeft, bottomRight]);

            return _bounds.Value.Item2;
        }

        public bool Contains(Vector2 point)
        {
            // Step 1: Translate the point to the rectangle's local space
            var translatedPoint = point - _target.Position;

            // Step 2: Reverse the rotation
            var cos = (float)Math.Cos(-_target.Rotation);
            var sin = (float)Math.Sin(-_target.Rotation);
            var rotatedPoint = new Vector2(
                translatedPoint.X * cos - translatedPoint.Y * sin,
                translatedPoint.X * sin + translatedPoint.Y * cos
            );

            // Step 3: Reverse the scale
            var scaledPoint = rotatedPoint / _target.Scale;

            var x = (int)(-_target.RelativeOrigin.X * _target.Dimensions.X);
            var y = (int)(-_target.RelativeOrigin.Y * _target.Dimensions.Y);
            var width = (int)_target.Dimensions.X;
            var height = (int)_target.Dimensions.Y;

            // Step 4: Check if the point is within the bounds of the rectangle
            var localRect = new Rectangle(x, y, width, height);

            return localRect.Contains((int)scaledPoint.X, (int)scaledPoint.Y);
        }

        private static Vector2 RotatePoint(Vector2 point, float angle)
        {
            var cos = (float)Math.Cos(angle);
            var sin = (float)Math.Sin(angle);

            return new Vector2(
                point.X * cos - point.Y * sin,
                point.X * sin + point.Y * cos
            );
        }

        private (BoundsCacheKey, Vector2[])? _bounds;

        private record BoundsCacheKey(Vector2 Position, Vector2 Dimensions, Vector2 Scale, float Rotation, Vector2 Origin);
    }
}
