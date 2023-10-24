﻿using System.Numerics;

namespace Hyjynx.Core.Rendering.Interface
{
    public class StackContainer : ContainerBase
    {
        public LayoutDirection Direction
        {
            get => _direction;
            set
            {
                _direction = value;
                Arrange();
                Measure();
            }
        }

        public Vector4 Padding
        {
            get => _padding;
            set
            {
                _padding = value;
                Arrange();
                Measure();
            }
        }

        protected override void Arrange()
        {
            var padding = Direction switch
            {
                LayoutDirection.Horizontal => new Vector2(Padding.Z, 0),
                LayoutDirection.Vertical => new Vector2(0, Padding.W),
                _ => throw new InvalidOperationException("Unknown LayoutDirection"),
            };

            var position = Vector2.Zero;

            foreach (var renderable in _renderables)
            {
                if (!renderable.IsVisible)
                {
                    continue;
                }

                if (renderable is not ITranslatable translatable)
                {
                    continue;
                }

                translatable.Offset = position + padding;

                position += padding;

                if (renderable is not IMeasurable measurable)
                {
                    continue;
                }

                switch (Direction)
                {
                    case LayoutDirection.Horizontal:
                        position += new Vector2(measurable.Dimensions.X, 0);
                        break;

                    case LayoutDirection.Vertical:
                        position += new Vector2(0, measurable.Dimensions.Y);
                        break;
                }
            }
        }

        protected override bool CanAddChild<T>(T child)
        {
            if (child is not IMeasurable)
            {
                return false;
            }

            return base.CanAddChild(child);
        }

        private LayoutDirection _direction;
        private Vector4 _padding;
    }
}
