using Hyjynx.Core.Rendering;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Numerics;

namespace Hyjynx.App.Xna
{
    public class XnaRenderTarget2D : RenderTarget2D, IRenderTarget
    {
        public string? Identifier { get; set; }

        public XnaRenderTarget2D(GraphicsDevice graphicsDevice, int width, int height)
            : base(graphicsDevice, width, height)
        {
            Identifier = $"{Guid.NewGuid()}";
        }


    }
}