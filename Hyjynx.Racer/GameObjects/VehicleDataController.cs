using Hyjynx.Core.Debugging;
using Hyjynx.Core.Rendering;
using Hyjynx.Core.Rendering.Interface;
using Hyjynx.Core.Services;
using System.Drawing;
using System.Numerics;

namespace Hyjynx.Racer.GameObjects
{
    public class VehicleDataController : StackContainer
    {
        private readonly IUtilityService _utilityService;

        public float Acceleration { get; set; } = .1f;
        public float TopSpeed { get; set; } = 10f;
        public float TopReverseSpeedFactor { get; set; } = .75f;
        public float DecelarationFactor { get; set; } = 1.5f;
        public float TurnSpeed { get; set; } = .03f;

        public VehicleDataController(
            IUtilityService utilityService
            )
        {
            Direction = LayoutDirection.Vertical;
                Padding = new Vector4(5);
                ZIndex = 2;

            _utilityService = utilityService;

            Add(AddVehicleDataControllerButtons("Acceleration", () => Acceleration, x => Acceleration = x, .01f));
            Add(AddVehicleDataControllerButtons("Top Speed", () => TopSpeed, x => TopSpeed = x, 1f));
            Add(AddVehicleDataControllerButtons("Reverse Factor", () => TopReverseSpeedFactor, x => TopReverseSpeedFactor = x, .05f));
            Add(AddVehicleDataControllerButtons("Decelaration Factor", () => DecelarationFactor, x => DecelarationFactor = x, .25f));
            Add(AddVehicleDataControllerButtons("Turn Speed", () => TurnSpeed, x => TurnSpeed = x, .01f));
        }

        private IContainer AddVehicleDataControllerButtons(string name, Func<float> getter, Action<float> setter, float increment)
        {
            IContainer container = new StackContainer
            {
                Direction = LayoutDirection.Horizontal,
                Padding = new Vector4(5)
            };

            container += new TextBox { Font = DebuggingService.Font, MinimumDimensions = new Vector2(200, 50), Color = Color.Black, Text = name };
            container += _utilityService.CreateButton("-", 50, ChangeValue(getter, setter, -increment));
            container += _utilityService.CreateButton("+", 50, ChangeValue(getter, setter, increment));
            var value = new TextBox { Font = DebuggingService.Font, Color = Color.Black };
            value.Bind(v => v.Text = $"{getter():0.000}");
            container += value;

            return container;
        }

        private static EventHandler ChangeValue(Func<float> getter, Action<float> setter, float increment)
        {
            return (s, e) =>
            {
                setter(getter() + increment);
            };
        }
    }
}
