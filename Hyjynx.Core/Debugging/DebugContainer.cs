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
            Padding = new Vector4(0, 5, 0, 5);
        }

        public void Initialize(float screenWidth, float screenHeight)
        {
            AddButton("Logger",  ToggleDebugInfo);
            AddButton("Snapshot", LogSnapshot);
            AddButton("Visuals",LogVisuals);
            AddToggleButton("Outlines", DebugFlag.RenderableOutlines);
            AddToggleButton("Backgrounds", DebugFlag.RenderContainerBackgrounds);
            AddToggleButton("Empties", DebugFlag.ShowEmptyContainers);

            var x = screenWidth - Dimensions.X;
            var y = screenHeight - Dimensions.Y;

            Offset = new Vector2(x, y);
        }

        private void LogVisuals()
        {
            DebuggingService.Describe(_visualRoot, _loggingService);
        }

        private void ToggleDebugInfo()
        {
            _loggingService.IsVisible = !_loggingService.IsVisible;
            _loggingService.Info($"Logger toggled: {_loggingService.IsVisible}");
        }

        private void LogSnapshot()
        {
            var renderableCount = _visualRoot.GetChildCount<IRenderable>(true);
            var updateableCount = _visualRoot.GetChildCount<IUpdateable>(true);

            _loggingService.Info("[BEGIN SNAPSHOT]");
            _loggingService.Info($"Renderables: {renderableCount}");
            _loggingService.Info($"Updateables: {updateableCount}");
            _loggingService.Info("[END SNAPSHOT]");
        }

        private void AddToggleButton(string label, DebugFlag flag)
        {
            AddButton(label, () => Toggle(flag));
        }

        private static void Toggle(DebugFlag flag)
        {
            var isEnabled = DebuggingService.IsEnabled(flag);
            DebuggingService.Set(flag, !isEnabled);
        }

        private void AddButton(string label, Action handler)
        {
            AddChild(_utilityService.CreateButton(label, 0, 0, 150, 25, (s, e) => handler()));
        }

        private readonly ILoggingService _loggingService;
        private readonly IUtilityService _utilityService;
        private readonly IContainer _visualRoot;
    }
}
