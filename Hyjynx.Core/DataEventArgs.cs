namespace Hyjynx.Core
{
    public class DataEventArgs<T> : EventArgs, IDataEventArgs<T>
    {
        public T Data { get; }

        public DataEventArgs(T data)
        {
            Data = data;
        }
    }
}
