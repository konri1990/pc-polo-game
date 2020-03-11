using System.Threading.Tasks;

namespace Server.Runner
{
    public interface IServerRunner
    {
        Task Run();
    }

    class ServerRunner : IServerRunner
    {
        public Task Run()
        {
            throw new System.NotImplementedException();
        }
    }
}