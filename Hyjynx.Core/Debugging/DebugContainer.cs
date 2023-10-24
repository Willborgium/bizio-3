using Hyjynx.Core.Rendering;
using Hyjynx.Core.Rendering.Interface;
using Hyjynx.Core.Services;
using System.Numerics;

namespace Hyjynx.Core.Debugging
{
    public class DebugContainer : StackContainer
    {
        public DebugContainer(
            ILoggingService loggingService,
            IUtilityService utilityService,
            IContainer visualRoot)
        {
            _loggingService = loggingService;
            _utilityService = utilityService;
            _visualRoot = visualRoot;
            ZIndex = int.MaxValue;
        }

        public void Initialize(float screenWidth, float screenHeight)
        {
            AddChild(_utilityService.CreateButton("Logger", ToggleDebugInfo));
            AddChild(_utilityService.CreateButton("Snapshot", LogSnapshot));
            AddChild(_utilityService.CreateButton("Outlines", ToggleDebuggingOutlines));
            AddChild(_utilityService.CreateButton("Visuals", LogVisuals));

            var x = screenWidth - Dimensions.X;
            var y = screenHeight - Dimensions.Y;

            Offset = new Vector2(x, y);
        }

        private void LogVisuals(object sender, EventArgs e)
        {
            DebuggingService.Describe(_visualRoot, _loggingService);
        }

        private void ToggleDebugInfo(object sender, EventArgs e)
        {
            _loggingService.IsVisible = !_loggingService.IsVisible;
            _loggingService.Info($"Logger toggled: {_loggingService.IsVisible}");
        }

        private void LogSnapshot(object sender, EventArgs e)
        {
            var renderableCount = _visualRoot.GetChildCount<IRenderable>(true);
            var updateableCount = _visualRoot.GetChildCount<IUpdateable>(true);

            _loggingService.Info("[BEGIN SNAPSHOT]");
            _loggingService.Info($"Renderables: {renderableCount}");
            _loggingService.Info($"Updateables: {updateableCount}");
            _loggingService.Info("[END SNAPSHOT]");
        }

        private void ToggleDebuggingOutlines(object sender, EventArgs e)
        {
            var isEnabled = DebuggingService.IsEnabled(DebugFlag.RenderableOutlines);
            DebuggingService.Set(DebugFlag.RenderableOutlines, !isEnabled);
        }

        private readonly ILoggingService _loggingService;
        private readonly IUtilityService _utilityService;
        private readonly IContainer _visualRoot;
    }
}
