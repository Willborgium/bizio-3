using Hyjynx.Battler.Scenes;
using Hyjynx.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Drawing;

namespace Hyjynx.Battler
{
    public static class Package
    {
        public static IServiceCollection AddBattlerServices(this IServiceCollection services) =>
            services
                .AddFirstScene<InitializationScene>()
                .AddScene<BattleScene>()
                .AddSingleton(new InitializationArguments(1920, 1080, true, Color.Green));
    }
}