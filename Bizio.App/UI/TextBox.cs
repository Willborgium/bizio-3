﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bizio.App.UI
{
    public class TextBox : IRenderable
    {
        public bool IsVisible { get; set; }

        public int ZIndex { get; set; }

        public SpriteFont Font { get; set; }

        public Vector2 Position { get; set; }

        public Color Color { get; set; }

        public string Text { get; set; }

        public void Render(SpriteBatch renderer)
        {
            renderer.DrawString(Font, Text ?? string.Empty, Position, Color);
        }
    }
}
