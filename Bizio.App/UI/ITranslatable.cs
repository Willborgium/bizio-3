using Microsoft.Xna.Framework;

namespace Bizio.App.UI
{
    public interface ITranslatable
    {
        IContainer Parent { get; set; }

        Vector2 Position { get; set; }
    }
}
