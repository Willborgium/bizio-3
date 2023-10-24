using System.Drawing;
using System.Numerics;
using Hyjynx.Core.Services;

namespace Hyjynx.Core.Rendering.Interface
{
    public class Button : UiComponent
    {
        public bool IsEnabled { get; set; }

        public string Text { get; set; }

        public EventArgs Args { get; set; }

        public event EventHandler Clicked;

        private readonly IInputService _inputService;

        public Button(ButtonMetadata metadata, IInputService inputService)
            : base()
        {
            _metadata = metadata;
            _state = ButtonState.Default;
            _inputService = inputService;
            IsEnabled = true;
        }

        public void SetDimensions(Vector2 dimensions)
        {
            _dimensions = dimensions;
        }

        public override void Update()
        {
            base.Update();

            if (!CanBeClicked())
            {
                _state = ButtonState.Default;
                return;
            }

            var mouse = _inputService.GetMouseState();

            if (mouse == null)
            {
                return;
            }

            ButtonState nextState = ButtonState.Default;

            if (Bounds.Contains(mouse.Position))
            {
                if (mouse.LeftButton == InputButtonState.Down)
                {
                    nextState = ButtonState.Clicked;
                }
                else
                {
                    if (_state == ButtonState.Clicked)
                    {
                        Clicked?.Invoke(this, Args);
                    }

                    nextState = ButtonState.Hovered;
                }
            }

            _state = nextState;
        }

        protected override void RenderInternal(IRenderer renderer)
        {
            var source = IsEnabled ? _metadata.DefaultSource : _metadata.DisabledSource;

            switch (_state)
            {
                case ButtonState.Hovered:
                    source = _metadata.HoveredSource;
                    break;
                case ButtonState.Clicked:
                    source = _metadata.ClickedSource;
                    break;
            }

            renderer.Draw(_metadata.Spritesheet, Destination, source, Color.White);

            var textDimensions = _metadata.Font.MeasureString(Text);

            var centerX = Destination.Left + (Destination.Width / 2);
            var x = centerX - textDimensions.X / 2;

            var centerY = Destination.Top + (Destination.Height / 2);
            var y = centerY - textDimensions.Y / 2;

            renderer.DrawText(_metadata.Font, Text, new Vector2(x, y), Color.Black);
        }

        protected override Vector2 GetDimensions() => _dimensions;

        private bool CanBeClicked()
        {
            if (!IsEnabled || !IsVisible) return false;

            var p = Parent;

            while (p != null)
            {
                if (!p.IsVisible) return false;

                p = p.Parent;
            }

            return true;
        }

        private ButtonState _state;
        private Vector2 _dimensions;
        private readonly ButtonMetadata _metadata;
    }
}
