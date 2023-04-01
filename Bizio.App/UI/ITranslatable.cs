using Microsoft.Xna.Framework;

namespace Bizio.App.UI
{
    public interface ITranslatable
    {
        // todo: gotta move this to some base class
        IContainer Parent { get; set; }

        Vector2 Position { get; set; }
    }
}
