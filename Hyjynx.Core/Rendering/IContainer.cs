using System.Numerics;

namespace Hyjynx.Core.Rendering
{
    public interface IContainer : IUpdateable, IRenderable, ITranslatable, IEnumerable<IIdentifiable>
    {
        void AddChild(IIdentifiable child);
        void Add(IIdentifiable child);
        void RemoveChild(IIdentifiable child);
        void RemoveChild(string identifier);
        int GetChildCount(bool isRecursive);
        int GetChildCount<T>(bool isRecursive);
        IIdentifiable FindChild(string identifier);
        T FindChild<T>(string identifier);
        Vector2 GetChildAbsolutePosition(ITranslatable child);
        event EventHandler ChildrenChanged;

        public static IContainer operator+(IContainer container, IIdentifiable child) { container.Add(child); return container; }
        public static IContainer operator-(IContainer container, IIdentifiable child) { container.RemoveChild(child); return container; }
        public static IContainer operator -(IContainer container, string identifier) { container.RemoveChild(identifier); return container; }
    }
}
