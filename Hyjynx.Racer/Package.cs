using Hyjynx.Core;
using Hyjynx.Racer.Scenes;
using Hyjynx.Racer.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Drawing;

namespace Hyjynx.Racer
{
    public static class Package
    {
        public static IServiceCollection AddRacerServices(this IServiceCollection services)
        {
            var width = 1920;
            var height = 1080;
            var scale = .9f;

            return services
                .AddFirstScene<InitializationScene>()
                .AddScene<MainMenuScene>()
                .AddScene<SetupNewGameScene>()
                .AddScene<TestGameplayScene>()
                .AddSingleton<ITrackService, TrackService>()
                .AddSingleton(new InitializationArguments((int)(width * scale), (int)(height * scale), true, Color.Orange));
        }
    }
}
