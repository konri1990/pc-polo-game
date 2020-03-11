using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Server
{
    class Server
    {
        private static IServiceProvider _serviceProvider;

        static async Task Main(string[] args)
        {
            RegisterServices();

            var service = _serviceProvider.GetService<IServerRunner>();
            try
            {
                await service.Run();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                DisposeServices();
            }
        }

        private static void RegisterServices()
        {
            var serviceCollection = new ServiceCollection();
            
            serviceCollection.AddLogging(configure => configure.AddConsole())
                 .AddTransient<Server>();

            serviceCollection.AddSingleton<IServerRunner, ServerRunner>();
            
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        private static void DisposeServices()
        {
            if (_serviceProvider == null)
            {
                return;
            }
            if (_serviceProvider is IDisposable)
            {
                ((IDisposable)_serviceProvider).Dispose();
            }
        }
    }
}
