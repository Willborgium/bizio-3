using Hyjynx.Core.Services;
using System.Numerics;

namespace Hyjynx.Core.Rendering.Interface
{
    public class ScrollContainer : ContainerBase
    {
        public override Vector2 Dimensions { get => _dimensions; set => _dimensions = value; }

        public LayoutDirection Direction { get; set; }

        public float Step { get; set; }

        public ScrollContainer(IInputService inputService)
        {
            _inputService = inputService;
            Step = 25;
        }

        public override void Update()
        {
            base.Update();

            var mouse = _inputService.GetMouseState();

            int? wheelValue = null;

            if (Bounds.Contains(mouse.Position))
            {
                wheelValue = mouse.WheelValue;
            }

            if (wheelValue == _lastWheelValue)
            {
                return;
            }

            if (wheelValue.HasValue && _lastWheelValue.HasValue)
            {
                var offset = _lastWheelValue.Value - wheelValue.Value;

                var speed = Step;

                if (offset == 0)
                {
                    speed = 0;
                }
                else if (offset < 0)
                {
                    speed *= -1;
                }

                if (Direction == LayoutDirection.Vertical)
                {
                    ChildOffset = new Vector2(0, ChildOffset.Y + speed);
                }
                else
                {
                    ChildOffset = new Vector2(ChildOffset.X + speed, 0);
                }
            }

            _lastWheelValue = wheelValue;
        }

        private Vector2 _dimensions;
        private readonly IInputService _inputService;

        private int? _lastWheelValue;
    }
}
