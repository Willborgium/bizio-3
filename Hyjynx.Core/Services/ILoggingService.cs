using Hyjynx.Core.Rendering;

namespace Hyjynx.Core.Services
{
    public interface ILoggingService : IRenderable
    {
        ILoggingService Error(string message);
        ILoggingService Info(string message);
        void Initialize(IFont font, ITexture2D pixel);
        ILoggingService Warning(string message);
    }
}