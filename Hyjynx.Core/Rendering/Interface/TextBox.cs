using System.Drawing;
using System.Numerics;

namespace Hyjynx.Core.Rendering.Interface
{
    public class TextBox : UiComponent, IMeasurable
    {
        public IFont Font { get; set; }

        public Color Color { get; set; }

        public string Text { get; set; }

        public Vector2 MinimumDimensions { get; set; } = Vector2.Zero;

        public TextBox()
            : base()
        {
        }

        protected override Vector2 GetDimensions()
        {
            var textDimensions = Font?.MeasureString(Text ?? string.Empty) ?? Vector2.Zero;

            return new Vector2(Math.Max(textDimensions.X, MinimumDimensions.X), Math.Max(textDimensions.Y, MinimumDimensions.Y));
        }

        protected override void RenderInternal(IRenderer renderer)
        {
            var position = Parent?.GetChildAbsolutePosition(this) ?? Offset;

            renderer.DrawText(Font, Text ?? string.Empty, position, Color);
        }
    }
}
