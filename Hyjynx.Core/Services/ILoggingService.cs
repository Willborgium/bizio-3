using Hyjynx.Core.Rendering;

namespace Hyjynx.Core.Services
{
    public interface ILoggingService : IRenderable
    {
        void Error(string message);
        void Info(string message);
        void Initialize(IFont font, ITexture2D pixel);
        void Warning(string message);
    }
}