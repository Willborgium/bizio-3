using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Bizio.App.UI
{
    public class Button : UiComponent, IMeasurable
    {
        public bool IsEnabled { get; set; }

        public string Text { get; set; }

        public Vector2 Dimensions { get; set; }

        public EventArgs Args { get; set; }

        public event EventHandler Clicked;

        public Button(ButtonMetadata metadata)
            : base()
        {
            _metadata = metadata;
            _state = ButtonState.Default;
            IsEnabled = true;
        }

        public override void Update()
        {
            base.Update();

            if (!CanBeClicked())
            {
                _state = ButtonState.Default;
                return;
            }

            var mouse = Mouse.GetState();

            ButtonState nextState = ButtonState.Default;

            if (Destination.Contains(mouse.Position))
            {
                if (mouse.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
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

        protected override void RenderInternal(SpriteBatch renderer)
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
            var x = Destination.Center.X - (textDimensions.X / 2);
            var y = Destination.Center.Y - (textDimensions.Y / 2);

            renderer.DrawString(_metadata.Font, Text, new Vector2(x, y), Color.Black);
        }

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

        private Rectangle Destination
        {
            get
            {
                var position = Parent?.GetChildAbsolutePosition(this) ?? Position;

                return new Rectangle((int)(position.X), (int)(position.Y), (int)Dimensions.X, (int)Dimensions.Y);
            }
        }

        private ButtonState _state;

        private readonly ButtonMetadata _metadata;
    }
}
