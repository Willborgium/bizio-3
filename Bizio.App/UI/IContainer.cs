using Microsoft.Xna.Framework;
using System;

namespace Bizio.App.UI
{
    public interface IContainer : IUpdateable, IRenderable, ITranslatable
    {
        void AddChild(IIdentifiable child);
        void RemoveChild(IIdentifiable child);
        void RemoveChild(string identifier);
        int GetChildCount(bool isRecursive);
        int GetChildCount<T>(bool isRecursive);
        IIdentifiable FindChild(string identifier);
        T FindChild<T>(string identifier);
        Vector2 GetChildAbsolutePosition(ITranslatable child);
        event EventHandler ChildrenChanged;
    }
}
