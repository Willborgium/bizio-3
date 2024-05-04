using Hyjynx.Core;
using Microsoft.Extensions.DependencyInjection;
using Hyjynx.Core.Scenes;
using Hyjynx.Tycoon.Scenes;
using System.Drawing;

namespace Hyjynx.Tycoon
{
    public static class Package
    {
        public static IServiceCollection AddTycoonServices(this IServiceCollection services)
        {
            var ratio = 1.1f;
            var width = 1920;
            var height = 1080;

            return services
                .AddFirstScene<DefaultInitializationScene<MainMenuScene>>()
                .AddScene<MainMenuScene>()
                .AddScene<NewGameScene>()
                .AddScene<GameScene>()
                .AddSingleton(new InitializationArguments((int)(width * ratio), (int)(height * ratio), true, Color.Beige));
        }
    }
}
