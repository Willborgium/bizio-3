using Hyjynx.Core.Debugging;
using System.Drawing;
using System.Numerics;

namespace Hyjynx.Core.Rendering
{
    public class Sprite : BindingBase, IRenderable, ITranslatable, IMeasurable, IRotatable
    {
        public bool IsVisible { get; set; }
        public int ZIndex { get; set; }
        public string? Identifier { get; set; }
        public IContainer? Parent { get; set; }
        public Vector2 Position { get; set; }

        public Vector2 Dimensions { get; set; }

        public Vector2 Scale { get; set; }

        public float Rotation { get; set; }
        public RotationAnchor Anchor
        {
            get => _anchor;
            set
            {
                _anchor = value;
                UpdateOrigin();
            }
        }

        public Vector2 Origin
        {
            get => _origin;
            set
            {
                if (Anchor != RotationAnchor.Custom)
                {
                    throw new InvalidOperationException($"Cannot explicitly set origin with Anchor set to '{Anchor}'. Please use type Custom.");
                }

                _origin = value;
            }
        }

        public Sprite(ITexture2D texture)
        {
            _texture = texture;
            Scale = Vector2.One;
            Dimensions = new Vector2(texture.Width, texture.Height);
        }

        public void Render(IRenderer renderer)
        {
            DebuggingService.DrawRectangle(renderer, Position, Dimensions * Scale);
            renderer.Draw(_texture, Position, Color.White, null, Rotation, _origin, Scale);
        }

        public Vector2 Translate(Vector2 offset) => Position += offset;
        public Vector2 Translate(float x, float y) => Position += new Vector2(x, y);
        public Vector2 Translate(int x, int y) => Position += new Vector2(x, y);

        private void UpdateOrigin()
        {
            var width = Dimensions.X;
            var height = Dimensions.Y;

            switch (Anchor)
            {
                case RotationAnchor.TopLeft:
                    _origin = new Vector2(0, 0);
                    break;
                case RotationAnchor.Center:
                    _origin = new Vector2(width / 2, height / 2);
                    break;
            }
        }

        private Vector2 _origin;
        private RotationAnchor _anchor;
        private readonly ITexture2D _texture;
    }
}
