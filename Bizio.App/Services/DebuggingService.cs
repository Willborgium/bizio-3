using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Bizio.App.Services
{
    public enum DebugFlag
    {
        None = 0,
        RenderableOutlines,
        ShowEmptyContainers
    }

    public static class DebuggingService
    {
        public static bool IsDebuggingEnabled { get; set; } = true;

        public static Texture2D PixelTexture { get; set; }

        public static SpriteFont Font { get; set; }

        static DebuggingService()
        {
            _flags = new Dictionary<DebugFlag, bool>
            {
                [DebugFlag.ShowEmptyContainers] = true,
            };
        }

        public static bool IsEnabled(params DebugFlag[] flags)
        {
            if (!IsDebuggingEnabled)
            {
                return false;
            }

            foreach (var flag in flags)
            {
                if (!_flags.ContainsKey(flag) ||
                    !_flags[flag])
                {
                    return false;
                }
            }

            return true;
        }

        public static void Set(DebugFlag flag, bool isEnabled) => _flags[flag] = isEnabled;

        public static void DrawRectangle(SpriteBatch renderer, Vector2 position, Vector2 dimensions)
        {
            if (!IsEnabled(DebugFlag.RenderableOutlines))
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

        public static void DrawEmptyContainerText(SpriteBatch renderer, Vector2 position)
        {
            if (!IsEnabled(DebugFlag.ShowEmptyContainers))
            {
                return;
            }

            renderer.DrawString(Font, "Empty Container", position, Color.Black);
        }

        private static readonly IDictionary<DebugFlag, bool> _flags;
    }
}
