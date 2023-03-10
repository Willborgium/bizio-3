using System;

namespace Bizio.App.UI
{
    public class DynamicDataEventArgs<T> : EventArgs, IDataEventArgs<T>
    {
        public T Data => _dataGetter.Invoke();

        public DynamicDataEventArgs(Func<T> dataGetter)
        {
            _dataGetter = dataGetter;
        }

        private readonly Func<T> _dataGetter;
    }
}
