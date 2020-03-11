using System.Threading.Tasks;
using Server.Runner;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace ServerTests.Runner
{
    public class MockServiceRegistry : ServiceRegistry
    {
        public override void ConfigureCustomServices(ServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IServerRunner, ServerImplementationWhichThrowsException>();
        }
    }

    public class ServerImplementationWhichThrowsException : IServerRunner
    {
        public Task Run() => throw new Exception("Unhandled exception. Application will not run.");
    }
}