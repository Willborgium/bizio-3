using Microsoft.Xna.Framework;

namespace Bizio.App.UI
{
    public interface IContainer : IRenderable, ITranslatable
    {
        void AddChild(IRenderable child);
        void RemoveChild(IRenderable child);
        Vector2 GetChildAbsolutePosition(ITranslatable child);
        int GetChildCount(bool isRecursive);
    }
}
