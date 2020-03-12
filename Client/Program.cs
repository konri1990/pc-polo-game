using System.Threading.Tasks;

namespace Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var bootstrapper = new Bootstrapper();
            await bootstrapper.Start(args);
        }
    }
}
