using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bizio.App.UI
{
    public abstract class UiComponent : IRenderable, ITranslatable, ILocatable
    {
        public bool IsVisible { get; set; }
        public int ZIndex { get; set; }
        public IContainer Parent { get; set; }
        public Vector2 Position { get; set; }
        public string Locator { get; set; }

        protected UiComponent()
        {
            IsVisible = true;
        }

        public void Render(SpriteBatch renderer)
        {
            if (!IsAbsolutelyVisible())
            {
                return;
            }

            RenderInternal(renderer);
        }

        protected abstract void RenderInternal(SpriteBatch renderer);

        private bool IsAbsolutelyVisible()
        {
            if (!IsVisible)
            {
                return false;
            }

            var parent = Parent;

            while (parent != null)
            {
                if (!parent.IsVisible)
                {
                    return false;
                }

                parent = parent.Parent;
            }

            return true;
        }
    }
}
