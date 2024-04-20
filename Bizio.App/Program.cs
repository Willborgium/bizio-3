using Hyjynx.Core;
using Hyjynx.Tycoon;
using Microsoft.Extensions.DependencyInjection;

namespace Hyjynx.App.Xna
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            new ServiceCollection()
                .AddCoreServices()
                .AddDriverServices()
                .AddTycoonServices()
                .RunDriver();
        }
    }
}