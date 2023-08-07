using Hyjynx.Bizio.Scenes;
using Hyjynx.Bizio.Services;
using Hyjynx.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Hyjynx.Core;

namespace Hyjynx.Bizio
{
    public static class Package
    {
        public static IServiceCollection AddBizioServices(this IServiceCollection services) =>
            services
                .AddSingleton<IDataService, DataService>()
                .AddSingleton<IUiService, UiService>()
                .AddFirstScene<BizioScene>()
                .AddSingleton(new InitializationArguments(1920, 1080));
    }
}
