using Microsoft.Xna.Framework;

namespace Bizio.App.UI
{
    public static class TranslatableExtensions
    {
        public static Vector2 ___GetAbsolutePosition(this ITranslatable t)
        {
            var position = t.Position;
            var p = t.Parent;

            while (p != null)
            {
                position += p.Position;
                p = p.Parent;
            }

            return position;
        }
    }
}
