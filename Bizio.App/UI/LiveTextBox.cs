using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Bizio.App.UI
{
    public class LiveTextBox : UiComponent, IMeasurable
    {
        public SpriteFont Font { get; set; }

        public Color Color { get; set; }

        public string Text => TextGetter?.Invoke() ?? string.Empty;

        public Func<string> TextGetter { get; set; }

        public Vector2 Dimensions => Font?.MeasureString(Text) ?? Vector2.Zero;

        public LiveTextBox()
            :base()
        {
        }

        protected override void RenderInternal(SpriteBatch renderer)
        {
            var position = Parent?.GetChildAbsolutePosition(this) ?? Position;

            renderer.DrawString(Font, Text, position, Color);
        }
    }
}
