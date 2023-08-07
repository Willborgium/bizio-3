namespace Hyjynx.Core.Services
{
    public interface IInputService : IUpdateable
    {
        event EventHandler<KeyStatesChangedEventArgs> KeyStatesChanged;
        event EventHandler<ValueChangedEventArgs<IMouseState>> MouseStateChanged;
        event EventHandler<ValueChangedEventArgs<IMouseState>> MousePositionChanged;
        event EventHandler<ValueChangedEventArgs<IMouseState>> MouseButtonChanged;
        event EventHandler<ValueChangedEventArgs<IMouseState>> MouseWheelChanged;

        IMouseState GetMouseState();
    }
}
