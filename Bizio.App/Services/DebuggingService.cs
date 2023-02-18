using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bizio.App.Services
{
    public static class DebuggingService
    {
        public static bool IsDebuggingEnabled { get; set; } = true;

        public static Texture2D PixelTexture { get; set; }

        public static void DrawRectangle(SpriteBatch renderer, Vector2 position, Vector2 dimensions)
        {
            if (!IsDebuggingEnabled)
            {
                return;
            }

            var thickness = 2;

            var top = new Rectangle((int)position.X, (int)position.Y, (int)dimensions.X, thickness);
            var bottom = new Rectangle((int)position.X, (int)(position.Y + dimensions.Y - thickness), (int)dimensions.X, thickness);
            var left = new Rectangle((int)position.X, (int)position.Y, thickness, (int)dimensions.Y);
            var right = new Rectangle((int)(position.X + dimensions.X - thickness), (int)position.Y, thickness, (int)dimensions.Y);

            renderer.Draw(PixelTexture, top, Color.White);
            renderer.Draw(PixelTexture, bottom, Color.White);
            renderer.Draw(PixelTexture, left, Color.White);
            renderer.Draw(PixelTexture, right, Color.White);
        }
    }
}
