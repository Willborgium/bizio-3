using Hyjynx.App.Xna;
using Hyjynx.Bizio;
using Hyjynx.Core;
using Microsoft.Extensions.DependencyInjection;

new ServiceCollection()
    .AddCoreServices()
    .AddDriverServices()
    .AddBizioServices()
    .RunDriver();