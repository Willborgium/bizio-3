using Bizio.App.UI;
using System;

namespace Bizio.App.Services
{
    public interface IUiService : IRenderable, IContainer
    {
        Button CreateButton(string text, int x, int y, int width, int height, EventHandler handler, EventArgs args = null);
        Button CreateButton<T>(string text, EventHandler<T> handler, T args = null)
            where T : EventArgs;
        Button CreateButton(string text, EventHandler handler, EventArgs args = null);
    }
}
