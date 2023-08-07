using System.Numerics;

namespace Hyjynx.Core.Rendering
{
    public interface ITranslatable
    {
        // todo: gotta move this to some base class
        IContainer? Parent { get; set; }

        Vector2 Position { get; set; }
    }
}
