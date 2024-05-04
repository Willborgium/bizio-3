namespace Hyjynx.Core.Rendering
{
    public interface IBindable : IUpdateable
    {
        ICollection<IUpdateable> Bindings { get; }
    }
}
