using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Server.Runner;

namespace Server
{
    public class TcpServer
    {
        private static IServiceProvider _serviceProvider;

        public static async Task Main(string[] args)
        {
            var serviceContainer = new ServiceRegistry();

            _serviceProvider = serviceContainer.RegisterServices();

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
