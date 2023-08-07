using Hyjynx.Core.Rendering.Interface;

namespace Hyjynx.Core.Services
{
    public interface IUtilityService
    {
        Button CreateButton(string text, int x, int y, int width, int height, EventHandler handler, EventArgs args = null);
        Button CreateButton<T>(string text, EventHandler<T> handler, T args = null)
            where T : EventArgs;
        Button CreateButton(string text, EventHandler handler, EventArgs args = null);
    }
}
