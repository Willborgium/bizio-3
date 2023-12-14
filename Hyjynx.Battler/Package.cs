using Hyjynx.Battler.Scenes;
using Hyjynx.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Drawing;

namespace Hyjynx.Battler
{
    public static class Package
    {
        public static IServiceCollection AddBattlerServices(this IServiceCollection services)
        {
            float ratio = .9f;
            var width = 1920;
            var height = 1080;
            return services
                .AddFirstScene<InitializationScene>()
                .AddScene<MainMenuScene>()
                .AddScene<GameWorldScene>()
                .AddScene<BattleScene>()
                .AddSingleton(new InitializationArguments((int)(width * ratio), (int)(height * ratio), true, Color.Green));
        }
    }
}