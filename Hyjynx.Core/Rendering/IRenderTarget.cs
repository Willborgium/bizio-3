using System.Numerics;

namespace Hyjynx.Core.Rendering
{
    public interface IRenderTarget : IDisposable, IIdentifiable
    {
        int Width { get; }
        int Height { get; }
    }
}
