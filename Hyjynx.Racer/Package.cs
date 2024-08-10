using Hyjynx.Core;
using Hyjynx.Racer.Scenes;
using Microsoft.Extensions.DependencyInjection;
using System.Drawing;

namespace Hyjynx.Racer
{
    public static class Package
    {
        public static IServiceCollection AddRacerServices(this IServiceCollection services) =>
            services
                .AddFirstScene<InitializationScene>()
                .AddScene<MainMenuScene>()
                .AddScene<SetupNewGameScene>()
                .AddScene<TestGameplayScene>()
                .AddSingleton(new InitializationArguments(1920, 1080, true, Color.Orange));
    }
}
