using Hyjynx.Core.Debugging;
using Hyjynx.Core.Rendering;
using Hyjynx.Core.Rendering.Interface;
using System.Drawing;
using System.Numerics;

namespace Hyjynx.Core.Services
{
    public class UtilityService : IUtilityService
    {
        public UtilityService(
            IResourceService resourceService,
            IInputService inputService,
            ILoggingService loggingService,
            InitializationArguments initializationArguments
            )
        {
            _resourceService = resourceService;
            _inputService = inputService;
            _loggingService = loggingService;
            _initializationArguments = initializationArguments;

            DebuggingService.IsDebuggingEnabled = _initializationArguments.IsDebugModeEnabled;
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

        public Button CreateButton(string text, int width, EventHandler handler, EventArgs args = null)
        {
            return CreateButton(text, 0, 0, width, 50, handler, args);
        }

        public void InitializeLogging(IContentService contentService)
        {
            var font = contentService.Load<IFont>("font-default");
            _resourceService.Set("font-default", font);
            DebuggingService.Font = font;

            var pixel = contentService.Load<ITexture2D>("pixel");
            _resourceService.Set("texture-pixel", pixel);
            DebuggingService.PixelTexture = pixel;

            _loggingService.Initialize(font, pixel);

            var buttonSpritesheet = contentService.Load<ITexture2D>("greySheet");
            var buttonMetadata = new ButtonMetadata
            {
                Font = font,
                Spritesheet = buttonSpritesheet,
                DefaultSource = new Rectangle(0, 143, 190, 45),
                HoveredSource = new Rectangle(0, 98, 190, 45),
                ClickedSource = new Rectangle(0, 188, 190, 49),
                DisabledSource = new Rectangle(0, 0, 195, 49)
            };

            _resourceService.Set("button-spritesheet-default", buttonSpritesheet);
            _resourceService.Set("button-metadata-default", buttonMetadata);
        }

        public void TryAddDebuggingContainer(IContainer root)
        {
            if (_initializationArguments.IsDebugModeEnabled)
            {
                var debugContainer = DebuggingService.CreateDebugContainer(_loggingService, this, root, _initializationArguments);
                debugContainer.Bind(c =>
                {
                    if (_inputService.GetKeyStates(Keys.LeftShift, Keys.OemTilde).All(k => k == KeyState.Pressed))
                    {
                        c.IsVisible = !c.IsVisible;
                    }
                });
                root.AddChild(debugContainer);
            }
        }

        private readonly IResourceService _resourceService;
        private readonly IInputService _inputService;
        private readonly ILoggingService _loggingService;
        private readonly InitializationArguments _initializationArguments;
    }
}
