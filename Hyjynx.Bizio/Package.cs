using Hyjynx.Bizio.Scenes;
using Hyjynx.Bizio.Services;
using Microsoft.Extensions.DependencyInjection;
using Hyjynx.Core;

namespace Hyjynx.Bizio
{
    public static class Package
    {
        public static IServiceCollection AddBizioServices(this IServiceCollection services) =>
            services
                .AddSingleton<IDataService, DataService>()
                .AddFirstScene<InitializationScene>()
                .AddScene<MainMenuScene>()
                .AddScene<BizioScene>()
                .AddSingleton(new InitializationArguments(1920, 1080, false));
    }
}
