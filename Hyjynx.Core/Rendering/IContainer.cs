using System.Numerics;

namespace Hyjynx.Core.Rendering
{
    public interface IContainer : IUpdateable, IRenderable, IMeasurable, IEnumerable<IIdentifiable>
    {
        Vector2 ChildOffset { get; }
        void AddChild(IIdentifiable child);
        void RemoveChild(IIdentifiable child);
        void RemoveChild(string identifier);
        int GetChildCount(bool isRecursive);
        int GetChildCount<T>(bool isRecursive);
        IIdentifiable FindChild(string identifier);
        T FindChild<T>(string identifier);
        event EventHandler ChildrenChanged;
    }
}
