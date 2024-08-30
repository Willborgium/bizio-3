using Hyjynx.Core.Rendering;
using Hyjynx.Core.Rendering.Interface;
using Hyjynx.Core.Services;
using System.Numerics;

namespace Hyjynx.Core.Debugging
{
    public class DebugContainer : VisualContainer
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
            var root = new StackContainer
            {
                Padding = Vector4.One * 10,
                Direction = LayoutDirection.Horizontal
            };

            AddChild(root);

            root.AddChild(_utilityService.CreateButton("Logger", 100, ToggleDebugInfo));
            root.AddChild(_utilityService.CreateButton("Snapshot", 100, LogSnapshot));
            root.AddChild(_utilityService.CreateButton("Outlines", 100, GetToggler(DebugFlag.RenderableOutlines)));
            root.AddChild(_utilityService.CreateButton("Identities", 100, GetToggler(DebugFlag.IdentifiableText)));
            root.AddChild(_utilityService.CreateButton("Visuals", 100, LogVisuals));

            var x = screenWidth - root.Dimensions.X;
            var y = screenHeight - root.Dimensions.Y;

            root.Position = new Vector2(x, y);
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

        private static EventHandler GetToggler(DebugFlag flag)
        {
            return (s, e) =>
            {
                var isEnabled = DebuggingService.IsEnabled(flag);
                DebuggingService.Set(flag, !isEnabled);
            };
        }

        private readonly ILoggingService _loggingService;
        private readonly IUtilityService _utilityService;
        private readonly IContainer _visualRoot;
    }
}
