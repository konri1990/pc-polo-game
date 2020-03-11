using System;
using System.Threading.Tasks;
using Server.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace Server
{
    public class Bootstrapper
    {
        public async Task Start(string[] args)
        {
            var serviceContainer = GetServiceRegistry;

            var serviceProvider = serviceContainer.RegisterServices();

            var server = serviceProvider.GetService<IServerRunner>();
            try
            {
                await server.Run();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                DisposeServices(serviceProvider);
            }
        }

        public virtual ServiceRegistry GetServiceRegistry => new ServiceRegistry();

        private static void DisposeServices(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                return;
            }
            if (serviceProvider is IDisposable)
            {
                ((IDisposable)serviceProvider).Dispose();
            }
        }
    }
}