using System;

namespace Bizio.App.UI
{
    public class DataEventArgs<T> : EventArgs
    {
        public T Data { get; }

        public DataEventArgs(T data)
        {
            Data = data;
        }
    }
}
