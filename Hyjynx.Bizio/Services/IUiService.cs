using Hyjynx.Core.Rendering;
using Hyjynx.Core.Rendering.Interface;
using System;

namespace Hyjynx.Bizio.Services
{
    public interface IUiService : IRenderable, IContainer
    {
        Button CreateButton(string text, int x, int y, int width, int height, EventHandler handler, EventArgs args = null);
        Button CreateButton<T>(string text, EventHandler<T> handler, T args = null)
            where T : EventArgs;
        Button CreateButton(string text, EventHandler handler, EventArgs args = null);
        TChild HideSiblings<TChild>(string identifier)
            where TChild : class, ITranslatable;
    }
}
