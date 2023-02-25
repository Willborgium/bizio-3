using Microsoft.Xna.Framework.Graphics;

namespace Bizio.App.Services
{
    public interface ILoggingService : IRenderable
    {
        void Error(string message);
        void Info(string message);
        void Initialize(SpriteFont font, Texture2D pixel);
        void Warning(string message);
    }
}