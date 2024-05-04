using System.Drawing;
using System.Numerics;

namespace Hyjynx.Core.Rendering
{
    public interface IRenderer
    {
        void Initialize();

        void Clear(Color color);

        void DrawText(IFont font, string text, Vector2 position, Color color);

        void Draw(
            ITexture2D texture,
            Vector2 position,
            Color? color = null,
            Rectangle? source = null,
            float rotation = 0f,
            Vector2? origin = null,
            Vector2? scale = null
        );

        void Draw(
            ITexture2D texture,
            Rectangle destination,
            Color? color = null,
            Rectangle? source = null,
            float rotation = 0f,
            Vector2? origin = null
        );

        void Begin2D();
        void End2D();
    }
}
