using System.Numerics;

namespace Hyjynx.Core.Rendering
{
    public static class TranslatableExtensions
    {
        public static Vector2 ___GetAbsolutePosition(this ITranslatable t)
        {
            var position = t.Offset;
            var p = t.Parent;

            while (p != null)
            {
                position += p.Offset;
                p = p.Parent;
            }

            return position;
        }
    }
}
