using Hyjynx.Core.Services;
using System.Drawing;

namespace Hyjynx.App.Xna
{
    public class XnaMouseStateProvider : IMouseStateProvider
    {
        public IMouseState GetMouseState()
        {
            var xnaState = Microsoft.Xna.Framework.Input.Mouse.GetState();
            var currentState = new MouseState
            {
                LeftButton = (InputButtonState)xnaState.LeftButton,
                RightButton = (InputButtonState)xnaState.RightButton,
                MiddleButton = (InputButtonState)xnaState.MiddleButton,
                WheelValue = xnaState.ScrollWheelValue,
                Position = new Point(xnaState.X, xnaState.Y)
            };

            if (_previousState != null)
            {
                currentState.LeftButton = GetButtonState(_previousState.LeftButton, currentState.LeftButton);
                currentState.RightButton = GetButtonState(_previousState.RightButton, currentState.RightButton);
                currentState.MiddleButton = GetButtonState(_previousState.MiddleButton, currentState.MiddleButton);
            }

            _previousState = currentState;

            return currentState;
        }

        private static InputButtonState GetButtonState(InputButtonState previous, InputButtonState current)
        {
            switch (previous)
            {
                case InputButtonState.Up:
                    if (current == InputButtonState.Down)
                    {
                        return InputButtonState.Pressed;
                    }
                    return InputButtonState.Up;
                case InputButtonState.Down:
                    if (current == InputButtonState.Up)
                    {
                        return InputButtonState.Released;
                    }
                    return InputButtonState.Down;
                case InputButtonState.Pressed:
                    if (current == InputButtonState.Down)
                    {
                        return InputButtonState.Down;
                    }
                    return InputButtonState.Released;
                case InputButtonState.Released:
                    if (current == InputButtonState.Up)
                    {
                        return InputButtonState.Up;
                    }
                    return InputButtonState.Pressed;
            }

            return current;
        }

        private MouseState _previousState;

        private class MouseState : IMouseState
        {
            public InputButtonState LeftButton { get; set; }
            public InputButtonState RightButton { get; set; }
            public InputButtonState MiddleButton { get; set; }

            public Point Position { get; set; }

            public int X => Position.X;
            public int Y => Position.Y;

            public int WheelValue { get; set; }
        }
    }
}
