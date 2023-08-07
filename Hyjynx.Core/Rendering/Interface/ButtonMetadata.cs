using System.Drawing;

namespace Hyjynx.Core.Rendering.Interface
{
    public class ButtonMetadata
    {
        public ITexture2D Spritesheet { get; set; }
        public IFont Font { get; set; }
        public Rectangle DefaultSource { get; set; }
        public Rectangle HoveredSource { get; set; }
        public Rectangle ClickedSource { get; set; }
        public Rectangle DisabledSource { get; set; }
    }
}
