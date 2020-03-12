using System;
using System.Threading.Tasks;
using Client.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Client
{
    public class Bootstrapper
    {
        public async Task Start(string[] args)
        {
            var serviceProvider = RegisterServices();

            var client = serviceProvider.GetService<IClientRunner>();
            var host = "127.0.0.1";
            var port = 8990;

            try
            {
                await client.Run(host, port);
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

        private IServiceProvider RegisterServices()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddLogging(configure => configure.AddConsole());
            
            ConfigureCustomServices(serviceCollection);

            return serviceCollection.BuildServiceProvider();
        }

        private void ConfigureCustomServices(ServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IClientRunner, ClientRunner>();
        }

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