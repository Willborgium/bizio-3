using Hyjynx.Core.Debugging;
using Hyjynx.Core.Rendering;
using Hyjynx.Core.Rendering.Interface;
using Hyjynx.Racer.Models;
using System.Drawing;
using System.Numerics;

namespace Hyjynx.Racer.GameObjects
{
    public class VehicleDebugger : StackContainer
    {
        public VehicleDebugger(Sprite car, Vehicle vehicle, Color? color = null)
        {
            Direction = LayoutDirection.Vertical;
            Padding = new Vector4(5);
            ZIndex = 5;

            color ??= Color.Black;

            var velocity = new LabeledTextBox { Font = DebuggingService.Font, Color = color.Value, Label = "Velocity" };
            AddChild(velocity);
            velocity.Bind(t => t.Text = $"{vehicle.Velocity:0.000}");

            var rotation = new LabeledTextBox { Font = DebuggingService.Font, Color = color.Value, Label = "Rotation" };
            AddChild(rotation);
            rotation.Bind(t => t.Text = $"{car.Rotation:0.000}");

            car.Bind(c => Position = c.Position - new Vector2(25, 200));

            var position = new LabeledTextBox { Font = DebuggingService.Font, Color = color.Value, Label = "Position" };
            AddChild(position);
            position.Bind(t => t.Text = $"{car.Position.X:0.0},{car.Position.Y:0.0}");
        }
    }
}
