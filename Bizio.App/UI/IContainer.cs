using Microsoft.Xna.Framework;

namespace Bizio.App.UI
{
    public interface IContainer : IUpdateable, IRenderable, ITranslatable
    {
        void AddChild<T>(T child);
        void RemoveChild<T>(T child);
        int GetChildCount(bool isRecursive);
        int GetChildCount<T>(bool isRecursive);
        ILocatable FindChild(string locator);
        Vector2 GetChildAbsolutePosition(ITranslatable child);
    }
}
