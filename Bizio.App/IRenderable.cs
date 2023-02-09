using Microsoft.Xna.Framework.Graphics;

namespace Bizio.App
{
    public interface IRenderable
    {
        bool IsVisible { get; set; }

        int ZIndex { get; set; }

        void Render(SpriteBatch renderer);
    }
}
