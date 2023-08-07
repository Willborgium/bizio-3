namespace Hyjynx.Core
{
    public class ValueChangedEventArgs<TValue> : EventArgs
    {
        public readonly TValue Previous;
        public readonly TValue Current;

        public ValueChangedEventArgs(TValue previous, TValue current)
        {
            Previous = previous;
            Current = current;
        }
    }
}
