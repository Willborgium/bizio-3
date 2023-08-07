namespace Hyjynx.Core
{
    public class Observer<T> : IUpdateable
        where T : struct
    {
        public event EventHandler<ValueChangedEventArgs<T>>? ValueChanged;

        public Observer(Func<T> subject)
        {
            _subject = subject;
            _value = default;
        }

        public void Update()
        {
            var currentValue = _subject();

            if (EqualityComparer<T>.Default.Equals(currentValue, _value))
            {
                return;
            }

            ValueChanged?.Invoke(this, new(_value, currentValue));

            _value = currentValue;
        }

        private readonly Func<T> _subject;

        private T _value;
    }
}
