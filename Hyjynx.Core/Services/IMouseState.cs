using System.Drawing;

namespace Hyjynx.Core.Services
{
    public interface IMouseState
    {
        InputButtonState LeftButton { get; }
        InputButtonState RightButton { get; }
        InputButtonState MiddleButton { get; }

        Point Position { get; }

        int X { get; }
        int Y { get; }

        int WheelValue { get; }
    }
}
