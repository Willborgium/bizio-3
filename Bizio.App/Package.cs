using Hyjynx.Core;
using Hyjynx.Core.Rendering;
using Hyjynx.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework.Graphics;

namespace Hyjynx.App.Xna
{
    public static class Package
    {
        public static IServiceCollection AddDriverServices(this IServiceCollection services) =>
            services
                .AddScoped<XnaWindowManagementService, XnaWindowManagementService>()
                .AddScoped<IWindowManagementService>(s => s.GetRequiredService<XnaWindowManagementService>())
                .AddScoped<IGraphicsDeviceManager>(s => s.GetRequiredService<XnaWindowManagementService>())
                .AddScoped<IGraphicsDeviceService>(s => s.GetRequiredService<XnaWindowManagementService>().GraphicsDeviceManager)
                .AddScoped<IContentService, XnaContentService>()
                .AddScoped<IRenderer, XnaRenderer>()
                .AddScoped<IKeyStateProvider, XnaKeyStateProvider>()
                .AddScoped<IMouseStateProvider, XnaMouseStateProvider>()
                .AddTransient<IDriver, Driver>();

        public static void RunDriver(this IServiceCollection services)
        {
            using (var provider = services.BuildServiceProvider())
            {
                provider.GetRequiredService<IDriver>().Run();
            }
        }
    }
}
