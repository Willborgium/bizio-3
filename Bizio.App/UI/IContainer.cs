using Microsoft.Xna.Framework;
using System;

namespace Bizio.App.UI
{
    public interface IContainer : IUpdateable, IRenderable, ITranslatable
    {
        void AddChild<T>(T child);
        void RemoveChild<T>(T child);
        int GetChildCount(bool isRecursive);
        int GetChildCount<T>(bool isRecursive);
        ILocatable FindChild(string locator);
        T FindChild<T>(string locator) where T : class, ILocatable;
        Vector2 GetChildAbsolutePosition(ITranslatable child);
        event EventHandler ChildrenChanged;
    }
}
