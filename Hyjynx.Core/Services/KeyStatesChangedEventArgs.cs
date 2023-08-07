namespace Hyjynx.Core.Services
{
    public class KeyStatesChangedEventArgs : EventArgs
    {
        public IReadOnlyDictionary<Keys, KeyState> KeyStates { get; }

        public KeyStatesChangedEventArgs(IReadOnlyDictionary<Keys, KeyState> keyStates)
        {
            KeyStates = keyStates;
        }

        public bool IsKey(Keys key, KeyState state)
        {
            return KeyStates[key] == state;
        }
    }
}
