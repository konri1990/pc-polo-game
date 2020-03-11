using System.Threading.Tasks;

public interface IServerRunner {
    Task Run();
}

class ServerRunner : IServerRunner
{
    public Task Run()
    {
        throw new System.NotImplementedException();
    }
}