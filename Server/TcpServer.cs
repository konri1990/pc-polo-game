using System.Threading.Tasks;

namespace Server
{
    public class TcpServer
    {
        public static async Task Main(string[] args)
        {
            var bootstrapper = new Bootstrapper();
            await bootstrapper.Start(args);
        }
    }
}
