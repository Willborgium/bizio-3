using Hyjynx.Core.Rendering;
using Hyjynx.Core.Rendering.Interface;
using Hyjynx.Core.Services;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace Hyjynx.Core.Debugging
{
    public static class DebuggingService
    {
        public static bool IsDebuggingEnabled { get; set; } = true;

        public static ITexture2D? PixelTexture { get; set; }

        public static IFont? Font { get; set; }

        static DebuggingService()
        {
            _flags = new Dictionary<DebugFlag, bool>
            {
                [DebugFlag.ShowEmptyContainers] = true,
            };
        }

        public static bool IsEnabled(params DebugFlag[] flags)
        {
            if (!IsDebuggingEnabled)
            {
                return false;
            }

            foreach (var flag in flags)
            {
                if (!_flags.ContainsKey(flag) ||
                    !_flags[flag])
                {
                    return false;
                }
            }

            return true;
        }

        public static void Set(DebugFlag flag, bool isEnabled) => _flags[flag] = isEnabled;

        public static void DrawRectangle(IRenderer renderer, Vector2 position, Vector2 dimensions)
        {
            if (!IsEnabled(DebugFlag.RenderableOutlines) || PixelTexture == null)
            {
                return;
            }

            var thickness = 2;

            var top = new Rectangle((int)position.X, (int)position.Y, (int)dimensions.X, thickness);
            var bottom = new Rectangle((int)position.X, (int)(position.Y + dimensions.Y - thickness), (int)dimensions.X, thickness);
            var left = new Rectangle((int)position.X, (int)position.Y, thickness, (int)dimensions.Y);
            var right = new Rectangle((int)(position.X + dimensions.X - thickness), (int)position.Y, thickness, (int)dimensions.Y);

            renderer.Draw(PixelTexture, top, Color.White);
            renderer.Draw(PixelTexture, bottom, Color.White);
            renderer.Draw(PixelTexture, left, Color.White);
            renderer.Draw(PixelTexture, right, Color.White);
        }

        public static void DrawEmptyContainerText(IRenderer renderer, Vector2 position)
        {
            if (!IsEnabled(DebugFlag.ShowEmptyContainers) || Font == null)
            {
                return;
            }

            renderer.DrawText(Font, "Empty Container", position, Color.Black);
        }

        public static void Describe(IIdentifiable root, ILoggingService loggingService)
        {
            var description = new StringBuilder();

            Describe(description, 0, root);

            var result = description.ToString();

            var filename = $"visual-tree-{DateTime.Now:yyyy-MM-dd-HH-mm-ss-ffff}.txt";

            using (var writer = new StreamWriter(filename))
            {
                writer.Write(result);
            }

            loggingService.Info($"Saved visual tree to file {filename}");
        }

        public static IContainer CreateDebugContainer(ILoggingService loggingService, IUtilityService utilityService, IContainer visualRoot, InitializationArguments initializationArguments)
        {
            var debugContainer = new DebugContainer(loggingService, utilityService, visualRoot);
            debugContainer.Initialize(initializationArguments.ScreenWidth, initializationArguments.ScreenHeight);
            return debugContainer;
        }

        private static void Describe(StringBuilder description, int depth, IIdentifiable node)
        {
            var indentation = new string(Enumerable.Repeat('\t', depth).ToArray());
            var id = Identify(node);
            var message = $"{indentation}{id}";

            description.AppendLine(message);

            if (node is IContainer container)
            {
                foreach (var child in container)
                {
                    Describe(description, depth + 1, child);
                }
            }
        }

        private static string Identify(IIdentifiable target)
        {
            if (!string.IsNullOrWhiteSpace(target.Identifier))
            {
                return target.Identifier;
            }

            var type = target.GetType().Name.ToString().ToLower();

            var description = "unknown";

            if (target is Button button)
            {
                description = button.Text ?? description;
            }
            else if (target is TextBox textBox)
            {
                description = textBox.Text ?? description;
            }
            else if (target is LabeledTextBox labeledTextBox)
            {
                description = labeledTextBox.Label ?? description;
            }

            description = description.Replace(" ", "-").ToLower();

            target.Identifier = $"{type}-{description}-{Guid.NewGuid()}";

            return target.Identifier;
        }

        private static readonly IDictionary<DebugFlag, bool> _flags;
    }
}
