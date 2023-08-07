using System.Drawing;
using System.Numerics;

namespace Hyjynx.Core.Rendering
{
    public interface IRenderer
    {
        void Initialize();

        void Clear(Color color);

        void Draw(ITexture2D texture, Rectangle destination, Color color);
        void Draw(ITexture2D texture, Rectangle destination, Rectangle source, Color color);
        void DrawText(IFont font, string text, Vector2 position, Color color);

        void Begin2D();
        void End2D();
    }
}
