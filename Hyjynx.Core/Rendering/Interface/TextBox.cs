using System.Drawing;
using System.Numerics;

namespace Hyjynx.Core.Rendering.Interface
{
    public class TextBox : UiComponent
    {
        public IFont Font { get; set; }

        public Color Color { get; set; }

        public string Text { get; set; }

        public TextBox()
            : base()
        {
        }

        protected override Vector2 GetDimensions() => Font?.MeasureString(Text ?? string.Empty) ?? Vector2.Zero;

        protected override void RenderInternal(IRenderer renderer)
        {
            var position = Offset;

            renderer.DrawText(Font, Text ?? string.Empty, position, Color);
        }
    }
}
