using Bizio.App.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Bizio.App
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var services = new ServiceCollection()
                .AddSingleton<IResourceService, ResourceService>()
                .AddSingleton<IDataService, DataService>()
                .AddSingleton<ILoggingService, LoggingService>()
                .AddSingleton<IUiService, UiService>()
                .AddTransient<IRunnable, BizioGame>();

            using (var provider = services.BuildServiceProvider())
            {
                provider
                    .GetRequiredService<IRunnable>()
                    .Run();
            }
        }
    }
}
