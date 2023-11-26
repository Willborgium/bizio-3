using System.Diagnostics;

namespace Hyjynx.Core.Services
{
    public class InputService : IInputService
    {
        public event EventHandler<KeyStatesChangedEventArgs> KeyStatesChanged;
        public event EventHandler<ValueChangedEventArgs<IMouseState>> MouseStateChanged;
        public event EventHandler<ValueChangedEventArgs<IMouseState>> MousePositionChanged;
        public event EventHandler<ValueChangedEventArgs<IMouseState>> MouseButtonChanged;
        public event EventHandler<ValueChangedEventArgs<IMouseState>> MouseWheelChanged;

        public InputService(
            IKeyStateProvider keyStateProvider,
            IMouseStateProvider mouseStateProvider)
        {
            _keyStateProvider = keyStateProvider;
            _mouseStateProvider = mouseStateProvider;
            _previousKeyStates = Enum.GetValues<Keys>().ToDictionary(k => k, k => KeyState.Up);
        }

        public void Update()
        {
            UpdateKeyStates();
            UpdateMouseState();
        }

        public IMouseState GetMouseState() => _previousMouseState;

        public KeyState GetKeyState(Keys key) => _previousKeyStates?[key] ?? KeyState.Up;

        public KeyState[] GetKeyStates(params Keys[] keys)
        {
            var states = new KeyState[keys.Length];

            for (var index = 0; index < keys.Length; index++)
            {
                states[index] = _previousKeyStates?[keys[index]] ?? KeyState.Up;
            }

            return states;
        }

        private void UpdateKeyStates()
        {
            var currentKeyStates = _keyStateProvider.GetKeyStates();

            var didStateChange = false;

            foreach ((var key, var currentState) in currentKeyStates)
            {
                var previousState = _previousKeyStates[key];

                var actualState = currentState;

                switch ((previousState, currentState))
                {
                    case (KeyState.Up, KeyState.Down):
                    case (KeyState.Released, KeyState.Down):
                        actualState = KeyState.Pressed;
                        break;
                    case (KeyState.Down, KeyState.Up):
                    case (KeyState.Pressed, KeyState.Up):
                        actualState = KeyState.Released;
                        break;
                }

                if (previousState != actualState)
                {
                    didStateChange = true;
                }

                _previousKeyStates[key] = actualState;
            }

            if (didStateChange)
            {
                KeyStatesChanged?.Invoke(this, new KeyStatesChangedEventArgs((IReadOnlyDictionary<Keys, KeyState>)_previousKeyStates));
            }
        }

        private void UpdateMouseState()
        {
            var currentMouseState = _mouseStateProvider.GetMouseState();

            if (_previousMouseState == null)
            {
                _previousMouseState = currentMouseState;
                return;
            }

            var mouseButtonChanged = currentMouseState.LeftButton != _previousMouseState.LeftButton ||
                currentMouseState.MiddleButton != _previousMouseState.MiddleButton ||
                currentMouseState.RightButton != _previousMouseState.RightButton;

            var mousePositionChanged = currentMouseState.X != _previousMouseState.X ||
                currentMouseState.Y != _previousMouseState.Y;

            var mouseWheelChanged = currentMouseState.WheelValue != _previousMouseState.WheelValue;

            if (!mouseButtonChanged && !mousePositionChanged && !mouseWheelChanged)
            {
                return;
            }

            var args = new ValueChangedEventArgs<IMouseState>(_previousMouseState, currentMouseState);

            MouseStateChanged?.Invoke(this, args);

            if (mouseButtonChanged)
            {
                MouseButtonChanged?.Invoke(this, args);
            }

            if (mouseWheelChanged)
            {
                MouseWheelChanged?.Invoke(this, args);
            }

            if (mousePositionChanged)
            {
                MousePositionChanged?.Invoke(this, args);
            }

            _previousMouseState = currentMouseState;
        }

        private readonly IKeyStateProvider _keyStateProvider;
        private readonly IMouseStateProvider _mouseStateProvider;

        private readonly IDictionary<Keys, KeyState> _previousKeyStates;
        private IMouseState _previousMouseState;
    }
}