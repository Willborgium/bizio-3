using Hyjynx.Core.Rendering;
using Hyjynx.Core.Services;
using System.Drawing;

namespace Hyjynx.Core
{
    public class DriverImplementation : IDriverImplementation
    {
        public event EventHandler Exit;

        public DriverImplementation(
            IWindowManagementService windowManagementService,
            IRenderer renderer,
            ISceneService sceneService,
            ILoggingService loggingService,
            IInputService inputService,
            InitializationArguments arguments)
        {
            _windowManagementService = windowManagementService;
            _renderer = renderer;
            _sceneService = sceneService;
            _loggingService = loggingService;
            _inputService = inputService;
            _arguments = arguments;
        }

        public void Initialize()
        {
            _windowManagementService.SetWindowDimensions(_arguments.ScreenWidth, _arguments.ScreenHeight);

            _renderer.Initialize();
        }

        public void Update()
        {
            _inputService.Update();

            if (!_sceneService.Update())
            {
                Exit(this, EventArgs.Empty);
            }
        }

        public void Draw()
        {
            _renderer.Begin2D();

            _renderer.Clear(Color.Coral);

            _sceneService.Render();

            _loggingService.Render(_renderer);

            _renderer.End2D();
        }

        private readonly IWindowManagementService _windowManagementService;
        private readonly IRenderer _renderer;
        private readonly ISceneService _sceneService;
        private readonly ILoggingService _loggingService;
        private readonly IInputService _inputService;
        private readonly InitializationArguments _arguments;
    }
}
