
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Server.Runner
{
    public class ServiceRegistry
    {
        public IServiceProvider RegisterServices()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddLogging(configure => configure.AddConsole());
            
            ConfigureCustomServices(serviceCollection);

            return serviceCollection.BuildServiceProvider();
        }

        public virtual void ConfigureCustomServices(ServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IServerRunner, ServerRunner>();
        }
    }
}