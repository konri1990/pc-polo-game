using System;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Server.Runner;
using Server;

namespace ServerTests.Runner
{
    public class ServerStartTests : Bootstrapper
    {
        [Fact]
        public async Task Given_ServerStart_When_RunMethodThrowException_Then_Rethrow()
        {
            Func<Task> runServer = async () => { await Start(null); };
            
            await runServer.Should().ThrowAsync<Exception>();
        }

        public override ServiceRegistry GetServiceRegistry => new MockServiceRegistry();
    }
}
