using Hyjynx.Core.Rendering;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace Hyjynx.App.Xna
{
    public class XnaRenderer : IRenderer
    {
        private const bool IS_DIAGNOSTIC_OUTPUT_ENABLED = false;

        public XnaRenderer(IGraphicsDeviceManager graphicsDeviceManager)
        {
            _graphicsDeviceManager = graphicsDeviceManager;
            _renderTargets = new Stack<IRenderTarget>();
        }

        public void Initialize()
        {
            _spriteBatch = new SpriteBatch(_graphicsDeviceManager.GraphicsDevice);

            var width = _graphicsDeviceManager.GraphicsDevice.PresentationParameters.BackBufferWidth;
            var height = _graphicsDeviceManager.GraphicsDevice.PresentationParameters.BackBufferHeight;

            var rt = CreateRenderTarget2D(width, height);

            _rootRenderTarget = rt as XnaRenderTarget2D;
            Diagnostic($"Root ID: {rt.Identifier}");
            _destination = new Rectangle(0, 0, width, height);
        }

        public void Clear(Color color)
        {
            Diagnostic($"Clear to {color}");

            var xColor = new Microsoft.Xna.Framework.Color(color.R, color.G, color.B, (byte)color.A);
            _graphicsDeviceManager.GraphicsDevice.Clear(xColor);
        }

        public void Draw(ITexture2D texture, Rectangle destination, Color color)
        {
            Diagnostic($"Draw texture at ({destination})");

            var xnaTexture = ToXna(texture);
            var xnaDestination = ToXna(destination);
            var xnaColor = ToXna(color);

            _spriteBatch.Draw(xnaTexture, xnaDestination, xnaColor);
        }

        public void Draw(ITexture2D texture, Rectangle destination, Rectangle source, Color color)
        {
            Diagnostic($"Draw sub-texture at ({destination})");

            var xnaTexture = ToXna(texture);
            var xnaDestination = ToXna(destination);
            var xnaSource = ToXna(source);
            var xnaColor = ToXna(color);

            _spriteBatch.Draw(xnaTexture, xnaDestination, xnaSource, xnaColor);
        }

        public void DrawText(IFont font, string text, Vector2 position, Color color)
        {
            Diagnostic($"Draw text '{text}' at ({position})");

            var xnaFont = ToXna(font);
            var xnaPosition = ToXna(position);
            var xnaColor = ToXna(Color.Blue);

            _spriteBatch.DrawString(xnaFont, text, xnaPosition, xnaColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public void Draw(IRenderTarget renderTarget, Rectangle destination, Color color)
        {
            Diagnostic($"Draw render target {renderTarget.Identifier} ({renderTarget.Width}, {renderTarget.Height}) at ({destination})");
            
            _spriteBatch.Draw(renderTarget as XnaRenderTarget2D, ToXna(destination), ToXna(color));
        }

        public IRenderTarget CreateRenderTarget2D(int width, int height)
        {
            var output = new XnaRenderTarget2D(_graphicsDeviceManager.GraphicsDevice, width, height);

            Diagnostic($"Creating render target {output.Identifier} ({width},{height})");

            return output;
        }

        public void PushRenderTarget(IRenderTarget renderTarget)
        {
            if (_renderTargets.TryPeek(out var lastRenderTarget))
            {
                EndSpriteBatch();
            }

            Diagnostic($"Set render target to {renderTarget.Identifier}");

            _graphicsDeviceManager.GraphicsDevice.SetRenderTarget(renderTarget as XnaRenderTarget2D);

            _renderTargets.Push(renderTarget);

            BeginSpriteBatch();
        }

        private void BeginSpriteBatch()
        {
            Diagnostic("Spritebatch begin");

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, DepthStencilState.Default, null, null, null);
        }

        private void EndSpriteBatch()
        {
            Diagnostic("Spritebatch end");

            _spriteBatch.End();
        }

        public IRenderTarget PopRenderTarget()
        {
            EndSpriteBatch();

            return _renderTargets.Pop();
        }

        public void Begin()
        {
            PushRenderTarget(_rootRenderTarget);
        }

        public void End()
        {
            while (_renderTargets.Any())
            {
                PopRenderTarget();
            }

            Diagnostic("Set render target to null");

            _graphicsDeviceManager.GraphicsDevice.SetRenderTarget(null);

            BeginSpriteBatch();

            Draw(_rootRenderTarget, _destination, Color.White);

            EndSpriteBatch();
        }

        private static Texture2D ToXna(ITexture2D texture)
        {
            return ((DynamicTexture2D)texture).As<Texture2D>();
        }

        private static Microsoft.Xna.Framework.Rectangle ToXna(Rectangle rectangle)
        {
            return new Microsoft.Xna.Framework.Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        private static Microsoft.Xna.Framework.Color ToXna(Color color)
        {
            return new Microsoft.Xna.Framework.Color(color.R, color.G, color.B, color.A);
        }

        private static SpriteFont ToXna(IFont font)
        {
            return ((DynamicFont)font).As<SpriteFont>();
        }

        private static Microsoft.Xna.Framework.Vector2 ToXna(Vector2 vector2)
        {
            return new Microsoft.Xna.Framework.Vector2(vector2.X, vector2.Y);
        }

        private static void Diagnostic(string message)
        {
            if (!IS_DIAGNOSTIC_OUTPUT_ENABLED)
            {
                return;
            }

            Debug.WriteLine(message);
        }

        private readonly IGraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;

        private XnaRenderTarget2D _rootRenderTarget;
        private Rectangle _destination;

        private readonly Stack<IRenderTarget> _renderTargets;
    }
}