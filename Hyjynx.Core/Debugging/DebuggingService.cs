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

        public static void Identify<T>(IRenderer renderer, T target, Vector2? position = null)
            where T : IIdentifiable, ITranslatable
        {
            if (!IsEnabled(DebugFlag.IdentifiableText) || PixelTexture == null)
            {
                return;
            }

            var id = target.Identifier ?? "Unknown";
            var p = position ?? target.Position;

            renderer.DrawText(Font, id, p, Color.White);
        }

        private const float Thickness = 2;
        private static Vector2 BorderThickness = new(Thickness);

        private static Vector2 RotateAroundOrigin(Vector2 position, Vector2 origin, float rotation)
        {
            // Translate the position to the origin
            Vector2 translatedPosition = position - origin;

            // Apply the rotation matrix
            float cosTheta = (float)Math.Cos(rotation);
            float sinTheta = (float)Math.Sin(rotation);

            float rotatedX = translatedPosition.X * cosTheta - translatedPosition.Y * sinTheta;
            float rotatedY = translatedPosition.X * sinTheta + translatedPosition.Y * cosTheta;

            // Translate the rotated position back to the original origin
            Vector2 rotatedPosition = new Vector2(rotatedX, rotatedY) + origin;

            return rotatedPosition;
        }

        public static void DrawRectangle(IRenderer renderer, Vector2 position, Vector2 dimensions, float rotation = 0)
        {
            if (!IsEnabled(DebugFlag.RenderableOutlines) || PixelTexture == null)
            {
                return;
            }

            // Center
            renderer.Draw(PixelTexture, position, Color.Black, null, 0, null, BorderThickness);

            var hScale = new Vector2(dimensions.X, BorderThickness.Y);

            var topPosition = -dimensions / 2;
            var topOffset = RotateAroundOrigin(topPosition, Vector2.Zero, rotation);
            renderer.Draw(PixelTexture, position + topOffset, Color.Red, null, rotation, null, hScale);

            var bottomPosition = new Vector2(-dimensions.X / 2, dimensions.Y / 2);
            var bottomOffset = RotateAroundOrigin(bottomPosition, Vector2.Zero, rotation);
            renderer.Draw(PixelTexture, position + bottomOffset, Color.Blue, null, rotation, null, hScale);

            var vScale = new Vector2(BorderThickness.X, dimensions.Y + Thickness);

            var leftOffset = RotateAroundOrigin(topPosition, Vector2.Zero, rotation);
            renderer.Draw(PixelTexture, position + leftOffset, Color.Green, null, rotation, null, vScale);

            var rightPosition = new Vector2(dimensions.X / 2, -dimensions.Y / 2);
            var rightOffset = RotateAroundOrigin(rightPosition, Vector2.Zero, rotation);
            renderer.Draw(PixelTexture, position + rightOffset, Color.Purple, null, rotation, null, vScale);
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

        public static ContainerBase CreateDebugContainer(ILoggingService loggingService, IUtilityService utilityService, IContainer visualRoot, InitializationArguments initializationArguments)
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
