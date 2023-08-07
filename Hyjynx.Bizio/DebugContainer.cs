using Hyjynx.Bizio.Services;
using Hyjynx.Core;
using Hyjynx.Core.Debugging;
using Hyjynx.Core.Rendering;
using Hyjynx.Core.Rendering.Interface;
using Hyjynx.Core.Services;
using System;
using System.Numerics;

namespace Hyjynx.Bizio
{
    public class DebugContainer : VisualContainer
    {
        public DebugContainer(
            ILoggingService loggingService,
            IUiService uiService)
        {
            _loggingService = loggingService;
            _uiService = uiService;
        }

        public void Initialize(float screenWidth, float screenHeight)
        {
            var root = new StackContainer
            {
                Padding = Vector4.One * 10,
                Direction = LayoutDirection.Horizontal
            };

            AddChild(root);

            root.AddChild(_uiService.CreateButton("Logger", ToggleDebugInfo));
            root.AddChild(_uiService.CreateButton("Snapshot", LogSnapshot));
            root.AddChild(_uiService.CreateButton("Outlines", ToggleDebuggingOutlines));
            root.AddChild(_uiService.CreateButton("Visuals", LogVisuals));

            var x = screenWidth - root.Dimensions.X;
            var y = screenHeight - root.Dimensions.Y;

            root.Position = new Vector2(x, y);
        }

        private void LogVisuals(object sender, EventArgs e)
        {
            DebuggingService.Describe(_uiService, _loggingService);
        }

        private void ToggleDebugInfo(object sender, EventArgs e)
        {
            _loggingService.IsVisible = !_loggingService.IsVisible;
            _loggingService.Info($"Logger toggled: {_loggingService.IsVisible}");
        }

        private void LogSnapshot(object sender, EventArgs e)
        {
            var renderableCount = _uiService.GetChildCount<IRenderable>(true);
            var updateableCount = _uiService.GetChildCount<IUpdateable>(true);

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
        private readonly IUiService _uiService;
    }
}
