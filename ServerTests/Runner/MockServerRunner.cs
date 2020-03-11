using System.Threading.Tasks;
using Server.Runner;
using System;

namespace ServerTests.Runner
{
    public class MockServerRunner : IServerRunner
    {
        public Task Run() => throw new Exception("Unhandled exception. Application will not run.");
    }
}