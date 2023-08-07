using Hyjynx.Core.Rendering;
using Hyjynx.Core.Rendering.Interface;
using Hyjynx.Core.Services;
using System;
using System.Numerics;

namespace Hyjynx.Bizio.Services
{
    public class UiService : VisualContainer, IUiService
    {
        public UiService(
            IResourceService resourceService,
            ILoggingService loggingService,
            IInputService inputService
            )
        {
            _resourceService = resourceService;
            _loggingService = loggingService;
            _inputService = inputService;
            IsVisible = true;
            Identifier = "root";
        }

        public Button CreateButton(string text, int x, int y, int width, int height, EventHandler handler, EventArgs args = null)
        {
            var metadata = _resourceService.Get<ButtonMetadata>("button-metadata-default");

            var button = new Button(metadata, _inputService)
            {
                Text = text,
                Position = new Vector2(x, y),
                Args = args ?? EventArgs.Empty
            };
            button.SetDimensions(new Vector2(width, height));

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

        public TChild HideSiblings<TChild>(string identifier)
            where TChild : class, ITranslatable
        {
            var child = FindChild<TChild>(identifier);
            if (child.Parent == null)
            {
                return child;
            }

            foreach (var sibling in child.Parent)
            {
                if (sibling == child)
                {
                    continue;
                }

                if (sibling is IRenderable r)
                {
                    r.IsVisible = false;
                }
            }

            return child;
        }

        private readonly IResourceService _resourceService;
        private readonly ILoggingService _loggingService;
        private readonly IInputService _inputService;
    }
}
