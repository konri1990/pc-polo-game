using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using FluentAssertions;
using Server.Runner;
using Server;

namespace ServerTests.Runner
{
    public class ServerStartTests : ServiceRegistry
    {
        [Fact]
        public async Task Given_ServerStart_When_RunMethodThrowException_Then_Rethrow()
        {
            Func<Task> runServer = async () => { await TcpServer.Main(null); };
            
            await runServer.Should().ThrowAsync<Exception>();
        }

        public override void ConfigureCustomServices(ServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IServerRunner, MockServerRunner>();
        }
    }
}
