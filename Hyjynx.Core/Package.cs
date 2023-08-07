using Hyjynx.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Hyjynx.Core
{
    public record InitializationArguments(int ScreenWidth, int ScreenHeight);

    public static class Package
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services) =>
            services
                .AddSingleton<IResourceService, ResourceService>()
                .AddSingleton<ILoggingService, LoggingService>()
                .AddSingleton<IInputService, InputService>()
                .AddSingleton<ISceneService, SceneService>()
                .AddSingleton<IUtilityService, UtilityService>()
                .AddSingleton<IDriverImplementation, DriverImplementation>();

        public static IServiceCollection AddScene<TScene>(this IServiceCollection services)
            where TScene : class, IScene
        {
            return services
                .AddTransient<TScene>()
                .AddSingleton<Func<TScene>>(x => () => x.GetService<TScene>());
        }

        public static IServiceCollection AddFirstScene<TScene>(this IServiceCollection services)
            where TScene : class, IFirstScene
        {
            return services
                .AddScene<TScene>()
                .AddSingleton<Func<IFirstScene>>(x => () => x.GetService<TScene>());
        }
    }
}
