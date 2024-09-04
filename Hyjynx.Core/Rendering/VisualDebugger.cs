using Hyjynx.Core.Debugging;
using Hyjynx.Core.Rendering.Interface;
using System.Drawing;
using System.Numerics;

namespace Hyjynx.Core.Rendering
{
    public class VisualDebugger : StackContainer, IDebugger, IRenderable
    {
        private readonly Color _color;

        public bool IsEnabled { get; set; }

        public VisualDebugger(Color? color = null)
        {
            Direction = LayoutDirection.Vertical;
            Padding = new Vector4(5);
            ZIndex = 5;

            _color = color ?? Color.Black;
        }

        public void BindTo<T>(T target)
            where T : ITranslatable, IBindable
        {
            target.Bind(t =>
            {
                Offset = t.Offset;
                Update();
            });
        }

        protected override void RenderInternal(IRenderer renderer)
        {
            if (!IsEnabled)
            {
                return;
            }

            base.RenderInternal(renderer);
        }

        public void AddLabel(string label, Func<string> valueGetter)
        {
            var labeledTextBox = new LabeledTextBox { Font = DebuggingService.Font, Color = _color, Label = label };
            AddChild(labeledTextBox);
            labeledTextBox.Bind(ltb => ltb.Text = valueGetter());
        }

        public static string Print(Vector2 value) => $"{value.X:0.0},{value.Y:0.0}";
        public static string Print(float value) => $"{value:0.00}";
        public static string Print(float value, int precision) => value.ToString(FloatPrecisions[precision]);

        private static readonly IReadOnlyDictionary<int, string> FloatPrecisions = new Dictionary<int, string>
        {
            { 1, "F1" },
            { 2, "F2" },
            { 3, "F3" },
            { 4, "F4" },
            { 5, "F5" },
            { 6, "F6" },
            { 7, "F7" },
            { 8, "F8" },
            { 9, "F9" }
        };
    }
}
