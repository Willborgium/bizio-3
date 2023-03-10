using Bizio.App.UI;
using Microsoft.Xna.Framework.Graphics;

namespace Bizio.App
{
    public interface IRenderable : IIdentifiable
    {
        bool IsVisible { get; set; }

        int ZIndex { get; set; }

        void Render(SpriteBatch renderer);
    }
}
