using Hyjynx.Core.Debugging;
using Hyjynx.Core.Rendering.Interface;
using System.Drawing;
using System.Numerics;

namespace Hyjynx.Core.Rendering
{
    public class SpriteDebugger : StackContainer
    {
        public SpriteDebugger(Sprite target, Color? color = null)
        {
            Direction = LayoutDirection.Vertical;
            Padding = new Vector4(5);
            ZIndex = 5;

            var c = color ?? Color.Black;

            target.Bind(t => Offset = t.Offset);

            AddLabel("Position:", c, t => Print(t, target.Position));
            AddLabel("Offset:", c, t => Print(t, target.Offset));
            AddLabel("Dimensions:", c, t => Print(t, target.Dimensions));
            AddLabel("Scale:", c, t => Print(t, target.Scale));
            AddLabel("Rotation:", c, t => Print(t, target.Rotation));
            AddLabel("Origin:", c, t => Print(t, target.Origin));

            AddLabel("Bounds:", c, t => PrintBounds(t, target));
        }

        private static void PrintBounds(LabeledTextBox t, Sprite target)
        {
            var bounds = target.GetBounds();

            t.Text = string.Join(Environment.NewLine, bounds.Select(c => $"{c.X:0.0},{c.Y:0.0}"));
        }

        private void AddLabel(string label, Color color, Action<LabeledTextBox> printer)
        {
            var x = new LabeledTextBox { Font = DebuggingService.Font, Color = color, Label = label };
            AddChild(x);
            x.Bind(printer);
        }

        private static void Print(LabeledTextBox t, Vector2 value)
        {
            t.Text = $"{value.X:0.0},{value.Y:0.0}";
        }

        private static void Print(LabeledTextBox t, float value)
        {
            t.Text = $"{value:0.00}";
        }
    }
}
