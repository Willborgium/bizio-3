using Hyjynx.Core.Rendering;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using System.Numerics;

namespace Hyjynx.App.Xna
{
    public class XnaRenderer : IRenderer
    {
        private readonly IGraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;

        public XnaRenderer(IGraphicsDeviceManager graphicsDeviceManager)
        {
            _graphicsDeviceManager = graphicsDeviceManager;
        }

        public void Initialize()
        {
            _spriteBatch = new SpriteBatch(_graphicsDeviceManager.GraphicsDevice);
        }

        public void Clear(Color color)
        {
            var xColor = new Microsoft.Xna.Framework.Color(color.R, color.G, color.B, color.A);
            _graphicsDeviceManager.GraphicsDevice.Clear(xColor);
        }

        public void Draw(ITexture2D texture, Rectangle destination, Color color)
        {
            var xnaTexture = ToXna(texture);
            var xnaDestination = ToXna(destination);
            var xnaColor = ToXna(color);

            _spriteBatch.Draw(xnaTexture, xnaDestination, xnaColor);
        }

        public void Draw(ITexture2D texture, Rectangle destination, Rectangle source, Color color)
        {
            var xnaTexture = ToXna(texture);
            var xnaDestination = ToXna(destination);
            var xnaSource = ToXna(source);
            var xnaColor = ToXna(color);

            _spriteBatch.Draw(xnaTexture, xnaDestination, xnaSource, xnaColor);
        }

        public void DrawText(IFont font, string text, Vector2 position, Color color)
        {
            var xnaFont = ToXna(font);
            var xnaPosition = ToXna(position);
            var xnaColor = ToXna(color);

            _spriteBatch.DrawString(xnaFont, text, xnaPosition, xnaColor);
        }

        public void Begin2D()
        {
            _spriteBatch.Begin();
        }

        public void End2D()
        {
            _spriteBatch.End();
        }

        private static Texture2D ToXna(ITexture2D texture)
        {
            return ((DynamicTexture2D)texture).As<Texture2D>();
        }

        private static Microsoft.Xna.Framework.Rectangle ToXna(Rectangle rectangle)
        {
            return new Microsoft.Xna.Framework.Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        private static Microsoft.Xna.Framework.Color ToXna(Color color)
        {
            return new Microsoft.Xna.Framework.Color(color.R, color.G, color.B, color.A);
        }

        private static Microsoft.Xna.Framework.Graphics.SpriteFont ToXna(IFont font)
        {
            return ((DynamicFont)font).As<Microsoft.Xna.Framework.Graphics.SpriteFont>();
        }

        private static Microsoft.Xna.Framework.Vector2 ToXna(Vector2 vector2)
        {
            return new Microsoft.Xna.Framework.Vector2(vector2.X, vector2.Y);
        }
    }
}