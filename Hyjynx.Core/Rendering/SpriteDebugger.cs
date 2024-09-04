using System.Drawing;

namespace Hyjynx.Core.Rendering
{
    public class SpriteDebugger : VisualDebugger
    {
        public SpriteDebugger(Sprite target, Color? color = null)
            : base(color)
        {
            BindTo(target);

            AddLabel("Position:", () => Print(target.Position));
            AddLabel("Offset:", () => Print(target.Offset));
            AddLabel("Dimensions:", () => Print(target.Dimensions));
            AddLabel("Scale:", () => Print(target.Scale));
            AddLabel("Rotation:", () => Print(target.Rotation));
            AddLabel("Origin:", () => Print(target.Origin));
            AddLabel("Bounds:", () => PrintBounds(target));
        }

        private string PrintBounds(Sprite target) => string.Join(Environment.NewLine, target.GetBounds().Select(Print));
    }
}
