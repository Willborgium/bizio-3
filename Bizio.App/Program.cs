using Hyjynx.Battler;
using Hyjynx.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Hyjynx.App.Xna
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            new ServiceCollection()
                .AddCoreServices()
                .AddDriverServices()
                .AddBattlerServices()
                .RunDriver();
        }
    }
}