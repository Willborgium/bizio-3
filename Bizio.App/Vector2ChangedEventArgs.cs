using Microsoft.Xna.Framework;
using System;

namespace Bizio.App
{
    public class Vector2ChangedEventArgs : EventArgs
    {
        private readonly Vector2 Previous;
        private readonly Vector2 Current;

        public Vector2ChangedEventArgs(Vector2 previous, Vector2 current)
        {
            Previous = previous;
            Current = current;
        }
    }
}
