using System.Drawing;
using System.Numerics;

namespace Hyjynx.Core.Rendering.Interface
{
    public class LabeledTextBox : UiComponent
    {
        public IFont Font { get; set; }

        public Color Color { get; set; }

        public string Text { get; set; }

        public string Label { get; set; }

        public float LabelWidth { get; set; }

        public float TextWidth { get; set; }

        public LabeledTextBox()
            : base()
        {
        }

        protected override void RenderInternal(IRenderer renderer)
        {
            var position = Offset;

            renderer.DrawText(Font, Label ?? string.Empty, position, Color);

            position += new Vector2(GetLabelWidth(), 0);

            renderer.DrawText(Font, Text ?? string.Empty, position, Color);
        }

        protected override Vector2 GetDimensions()
        {
            var width = GetLabelWidth();

            if (!string.IsNullOrWhiteSpace(Text))
            {
                width += Font.MeasureString(Text).X;
            }

            var height = Font.LineSpacing;

            return new Vector2(width, height);
        }

        private float GetLabelWidth()
        {
            var width = 0f;

            if (LabelWidth > 0f)
            {
                width += LabelWidth;
            }
            else if (!string.IsNullOrWhiteSpace(Label))
            {
                width += Font.MeasureString(Label).X;
            }

            if (width > 0)
            {
                width += 5f; // padding
            }

            return width;
        }
    }
}
