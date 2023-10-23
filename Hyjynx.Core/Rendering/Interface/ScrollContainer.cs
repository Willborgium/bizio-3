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

            if (Destination.Contains(mouse.Position))
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
                    _offset = new Vector2(0, _offset.Y + speed);
                }
                else
                {
                    _offset = new Vector2(_offset.X + speed, 0);
                }
            }

            _lastWheelValue = wheelValue;
        }

        protected Vector2 _offset = Vector2.Zero;

        private Vector2 _dimensions;
        private readonly IInputService _inputService;

        private int? _lastWheelValue;
    }
}
