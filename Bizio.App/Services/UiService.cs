using Bizio.App.UI;
using Microsoft.Xna.Framework;
using System;

namespace Bizio.App.Services
{
    public class UiService : VisualContainer, IUiService
    {
        public UiService(IResourceService resourceService)
        {
            _resourceService = resourceService;
            IsVisible = true;
        }

        public Button CreateButton(string text, int x, int y, int width, int height, EventHandler handler, EventArgs args = null)
        {
            var metadata = _resourceService.Get<ButtonMetadata>("button-metadata-default");

            var button = new Button(metadata)
            {
                Text = text,
                Position = new Vector2(x, y),
                Dimensions = new Vector2(width, height),
                Args = args ?? EventArgs.Empty
            };

            button.Clicked += handler;

            return button;
        }

        public Button CreateButton<T>(string text, EventHandler<T> handler, T args = null)
            where T : EventArgs
        {
            return CreateButton(text, 0, 0, 300, 50, (s, e) => handler?.Invoke(s, e as T), args);
        }

        public Button CreateButton(string text, EventHandler handler, EventArgs args = null)
        {
            return CreateButton(text, 0, 0, 300, 50, handler, args);
        }

        private readonly IResourceService _resourceService;
    }
}
