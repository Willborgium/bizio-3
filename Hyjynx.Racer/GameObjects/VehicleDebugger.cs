using Hyjynx.Core.Debugging;
using Hyjynx.Core.Rendering;
using Hyjynx.Core.Rendering.Interface;
using System.Drawing;
using System.Numerics;

namespace Hyjynx.Racer.GameObjects
{
    public class VehicleDebugger : StackContainer
    {
        public VehicleDebugger(Sprite car, Vehicle vehicle)
        {
            Direction = LayoutDirection.Vertical;
            Padding = new Vector4(5);
            var font = DebuggingService.Font;

            var velocity = new LabeledTextBox { Font = font, Color = Color.Black, Label = "Velocity" };
            AddChild(velocity);
            velocity.Bind(t => t.Text = $"{vehicle.Velocity:0.000}");

            var rotation = new LabeledTextBox { Font = font, Color = Color.Black, Label = "Rotation" };
            AddChild(rotation);
            rotation.Bind(t => t.Text = $"{car.Rotation:0.000}");

            car.Bind(MoveStats);
        }

        private void MoveStats(Sprite car)
        {
            Position = car.Position - new Vector2(25, 200);
        }
    }
}
