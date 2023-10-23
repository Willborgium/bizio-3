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

        void Draw(IRenderTarget renderTarget, Rectangle destination, Color color);

        IRenderTarget CreateRenderTarget2D(int width, int height);

        void PushRenderTarget(IRenderTarget target);
        IRenderTarget PopRenderTarget();

        void Begin();
        void End();
    }
}
