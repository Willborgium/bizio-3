using Hyjynx.Core.Rendering.Interface;
using System.Numerics;

namespace Hyjynx.Core.Services
{
    public class UtilityService : IUtilityService
    {
        public UtilityService(
            IResourceService resourceService,
            IInputService inputService
            )
        {
            _resourceService = resourceService;
            _inputService = inputService;
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

        private readonly IResourceService _resourceService;
        private readonly IInputService _inputService;
    }
}
