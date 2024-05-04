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

        public void Draw(
            ITexture2D texture,
            Rectangle destination,
            Color? color = null,
            Rectangle? source = null,
            float rotation = 0f,
            Vector2? origin = null
            )
        {
            var xnaTexture = ToXna(texture);
            var xnaDestination = ToXna(destination);
            var xnaSource = ToXna(source) ?? xnaTexture.Bounds;
            var xnaColor = ToXna(color ?? Color.White);
            var xnaOrigin = ToXna(origin ?? Vector2.Zero);

            _spriteBatch.Draw(xnaTexture, xnaDestination, xnaSource, xnaColor, rotation, xnaOrigin, SpriteEffects.None, 0);
        }

        public void Draw(
            ITexture2D texture,
            Vector2 position,
            Color? color = null,
            Rectangle? source = null,
            float rotation = 0f,
            Vector2? origin = null,
            Vector2? scale = null
            )
        {
            var xnaTexture = ToXna(texture);
            var xnaPosition = ToXna(position);
            var xnaSource = ToXna(source) ?? xnaTexture.Bounds;
            var xnaColor = ToXna(color ?? Color.White);
            var xnaOrigin = ToXna(origin ?? Vector2.Zero);
            var xnaScale = ToXna(scale ?? Vector2.Zero);

            _spriteBatch.Draw(xnaTexture, xnaPosition, xnaSource, xnaColor, rotation, xnaOrigin, xnaScale, SpriteEffects.None, 0);
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

        private static Microsoft.Xna.Framework.Rectangle? ToXna(Rectangle? rectangle)
        {
            if (!rectangle.HasValue)
            {
                return null;
            }

            return new Microsoft.Xna.Framework.Rectangle(
                rectangle.Value.X,
                rectangle.Value.Y,
                rectangle.Value.Width,
                rectangle.Value.Height
            );
        }

        private static Microsoft.Xna.Framework.Color ToXna(Color color)
        {
            return new Microsoft.Xna.Framework.Color(color.R, color.G, color.B, color.A);
        }

        private static Microsoft.Xna.Framework.Color? ToXna(Color? color)
        {
            if (!color.HasValue)
            {
                return null;
            }

            return new Microsoft.Xna.Framework.Color(color.Value.R, color.Value.G, color.Value.B, color.Value.A);
        }

        private static SpriteFont ToXna(IFont font)
        {
            return ((DynamicFont)font).As<SpriteFont>();
        }

        private static Microsoft.Xna.Framework.Vector2 ToXna(Vector2 vector2)
        {
            return new Microsoft.Xna.Framework.Vector2(vector2.X, vector2.Y);
        }

        private static Microsoft.Xna.Framework.Vector2? ToXna(Vector2? vector2)
        {
            if (!vector2.HasValue)
            {
                return null;
            }

            return new Microsoft.Xna.Framework.Vector2(vector2.Value.X, vector2.Value.Y);
        }
    }
}