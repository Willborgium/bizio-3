using Hyjynx.Core;
using Hyjynx.Core.Services;
using Microsoft.Xna.Framework;

namespace Hyjynx.App.Xna
{
    public class Driver : Game, IDriver
    {
        public Driver(
            IWindowManagementService windowManagementService,
            IDriverImplementation driverImplementation
            )
        {
            _driverImplementation = driverImplementation;

            windowManagementService.Initialize(this);
        }

        protected override void Initialize()
        {
            _driverImplementation.Exit += (s, e) => Exit();

            IsMouseVisible = true;

            _driverImplementation.Initialize();

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            _driverImplementation.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _driverImplementation.Draw();

            base.Draw(gameTime);
        }

        private readonly IDriverImplementation _driverImplementation;
    }
}