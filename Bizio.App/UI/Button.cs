using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.ComponentModel.Design.Serialization;

namespace Bizio.App.UI
{
    public class Button : IUpdateable, IRenderable, ITranslatable, IMeasurable
    {
        public bool IsVisible { get; set; }

        public int ZIndex { get; set; }

        public bool IsEnabled { get; set; }

        public string Text { get; set; }

        public IContainer Parent { get; set; }

        public Vector2 Position { get; set; }

        public Vector2 Dimensions { get; set; }

        public event EventHandler Clicked;

        public Button(ButtonMetadata metadata)
        {
            _metadata = metadata;
            _state = ButtonState.Default;
            IsVisible = true;
            IsEnabled = true;
        }

        public void Update()
        {
            if (!IsEnabled)
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
                        Clicked?.Invoke(this, EventArgs.Empty);
                    }

                    nextState = ButtonState.Hovered;
                }
            }

            _state = nextState;
        }

        public void Render(SpriteBatch renderer)
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
