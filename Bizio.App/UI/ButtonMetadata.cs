using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bizio.App.UI
{
    public class ButtonMetadata
    {
        public Texture2D Spritesheet { get; set; }
        public SpriteFont Font { get; set; }
        public Rectangle DefaultSource { get; set; }
        public Rectangle HoveredSource { get; set; }
        public Rectangle ClickedSource { get; set; }
        public Rectangle DisabledSource { get; set; }
    }
}
