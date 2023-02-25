using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bizio.App.UI
{
    public class TextBox : IRenderable, ITranslatable, IMeasurable
    {
        public bool IsVisible { get; set; }

        public int ZIndex { get; set; }

        public IContainer Parent { get; set; }

        public SpriteFont Font { get; set; }

        public Vector2 Position { get; set; }

        public Color Color { get; set; }

        public string Text { get; set; }

        public Vector2 Dimensions => Font?.MeasureString(Text ?? string.Empty) ?? Vector2.Zero;

        public TextBox()
        {
            IsVisible = true;
        }

        public void Render(SpriteBatch renderer)
        {
            var position = Parent?.GetChildAbsolutePosition(this) ?? Position;

            renderer.DrawString(Font, Text ?? string.Empty, position, Color);
        }
    }
}
