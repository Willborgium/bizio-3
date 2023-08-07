using Hyjynx.Core.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Hyjynx.App.Xna
{
    public class XnaWindowManagementService : IWindowManagementService, IGraphicsDeviceManager, IDisposable
    {
        public GraphicsDevice GraphicsDevice => GraphicsDeviceManager.GraphicsDevice;

        public GraphicsDeviceManager GraphicsDeviceManager { get; private set; }

        public void Initialize(object caller)
        {
            GraphicsDeviceManager = new GraphicsDeviceManager(caller as Game);
        }

        public void SetWindowDimensions(int width, int height)
        {
            GraphicsDeviceManager.PreferredBackBufferWidth = width;
            GraphicsDeviceManager.PreferredBackBufferHeight = height;
            GraphicsDeviceManager.ApplyChanges();
        }

        #region IDisposable

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    GraphicsDeviceManager.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
