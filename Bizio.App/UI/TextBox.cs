using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bizio.App.UI
{
    public class TextBox : UiComponent, IMeasurable
    {
        public SpriteFont Font { get; set; }

        public Color Color { get; set; }

        public string Text { get; set; }

        public Vector2 Dimensions => Font?.MeasureString(Text ?? string.Empty) ?? Vector2.Zero;

        public TextBox()
            : base()
        {
        }

        protected override void RenderInternal(SpriteBatch renderer)
        {
            var position = Parent?.GetChildAbsolutePosition(this) ?? Position;

            renderer.DrawString(Font, Text ?? string.Empty, position, Color);
        }
    }
}
