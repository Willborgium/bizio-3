using Microsoft.Xna.Framework;
using System;

namespace Bizio.App.UI
{
    public interface ITranslatable
    {
        ITranslatable Parent { get; set; }

        Vector2 Position { get; set; }
    }
}
